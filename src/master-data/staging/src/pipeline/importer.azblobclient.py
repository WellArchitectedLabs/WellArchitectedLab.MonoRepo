"This implementation is done only for comparing the development experience between""""
Implementing csv access using kubernetes CSI driver, which would mount directly file via persistence volumes
And implementing integration using classic Blob client library (which can be done in kubernetes context too)
"""
import csv
import os
import time
from datetime import date, timedelta
from typing import Optional
from dateutil.relativedelta import relativedelta
from tqdm import tqdm
from config import WF_IMPORT_CSV_INPUT_READ_BATCH_SIZE
from azure.storage.blob import BlobServiceClient

from open_meteo import fetch_hourly
from config import WF_IMPORT_OUTPUT_CSV
from config import OPEN_METEO_BATCH_SIZE
from config import OPEN_METEO_THROTTLE_SECONDS
from config import WF_IMPORT_TIMEZONE
from config import OPEN_METEO_MAX_RETRIES
from config import OPEN_METEO_HOURLY_VARS 
from config import AZ_BLOB_CONTAINER, AZ_BLOB_CONNECTION_STRING


class _NoopProgress:
    def __init__(self, *a, **kw):
        pass

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc, tb):
        return False

    def update(self, n=1):
        return

def _read_csv_from_blob(blob_name: str):
    """
    Read CSV from Azure Blob Storage using connection string from environment variable.
    """
    if not AZ_BLOB_CONNECTION_STRING:
        raise SystemExit("AZURE_STORAGE_CONNECTION_STRING environment variable not set")

    blob_service_client = BlobServiceClient.from_connection_string(AZ_BLOB_CONNECTION_STRING)
    container_client = blob_service_client.get_container_client(AZ_BLOB_CONTAINER)
    blob_client = container_client.get_blob_client(blob_name)

    stream = blob_client.download_blob()
    text = stream.readall().decode("utf-8")
    reader = csv.DictReader(text.splitlines())
    return list(reader)

def load_cities_from_csv(path: str):
    locations = []

    # Detect if path is from Azure Blob Storage
    if path.startswith("blob://"):
        blob_name = path[len("blob://"):]
        rows = _read_csv_from_blob(blob_name)
        for row in rows:
            locations.append((
                float(row["Latitude"]),
                float(row["Longitude"]),
                row["Name"]
            ))
    else:
        with open(path, newline="", encoding="utf-8") as f:
            reader = csv.DictReader(f)
            for row in reader:
                locations.append((
                    float(row["Latitude"]),
                    float(row["Longitude"]),
                    row["Name"]
                ))
    return locations

def load_cities_from_db(db_dsn: str):
    # Expect db_adapter.read_cities to return list of dicts with latitude/longitude
    from adapters import WeatherForecastPgDbAdapter
    db_adapter = WeatherForecastPgDbAdapter(db_dsn)
    rows = db_adapter.read_all_cities()
    # return list of tuples (id, latitude, longitude)
    return [(r["id"], r["latitude"], r["longitude"]) for r in rows]

def chunked(seq, size):
    for i in range(0, len(seq), size):
        yield seq[i:i + size]

def month_ranges_between(start, end):
    ranges = []
    current = start.replace(day=1)

    while current <= end:
        month_end = (
            current + relativedelta(months=1)
        ) - timedelta(days=1)

        ranges.append((
            max(start, current),
            min(end, month_end)
        ))

        current += relativedelta(months=1)

    return ranges

def import_wf_actuals_from_open_meteo(
        from_date: date, 
        to_date: date, 
        db_dsn: str, 
        cities_csv_input: Optional[str] = None, 
        export_to_csv: bool = False, 
        export_to_postgres: bool = True) -> None:

    # guard against missing or incompatible inputs
    if db_dsn.strip() == "" and (cities_csv_input.strip() == "" or cities_csv_input is None):
        raise SystemError("No cities csv input is provided, neither a connection string. Cities cannot be loaded. Please provide a valid input.")

    # guard against incompatible database inputs
    if(db_dsn.strip() == "" and export_to_postgres):
        raise SystemError("Exporting to Postgres is not supported without a valid db connection string. Please provide a valid connection string.")
    
    if(from_date >= to_date):
        raise SystemError("Please provide a valid from date that is stricly inferior to to date.")

    locations = load_locations(db_dsn, cities_csv_input)

    months = month_ranges_between(from_date, to_date)

    total_requests = (
        ((len(locations) + OPEN_METEO_BATCH_SIZE - 1) // OPEN_METEO_BATCH_SIZE)
        * len(months)
    )

    from adapters import WeatherForecastPgDbAdapter 
    if db_dsn.strip() != "":
        db_adapter = WeatherForecastPgDbAdapter(db_dsn)

    # prepare CSV writer if requested
    csv_out = None
    if export_to_csv:
        csv_out = open(WF_IMPORT_OUTPUT_CSV, "w", newline="", encoding="utf-8")
        writer = csv.writer(csv_out)
        writer.writerow([
            "longitude",
            "latitude",
            "timestamp_utc",
            "temperature_c",
            "wind_speed_m_s",
            "precipitation_mm"
        ])

    # buffer rows for DB bulk insert
    db_buffer = []

    pbar_ctx = tqdm(total=total_requests, desc="Fetching Open-Meteo data", unit="request") if tqdm else _NoopProgress()
    with pbar_ctx as pbar:

            for batch in chunked(locations, OPEN_METEO_BATCH_SIZE):
                # support two shapes:
                # - CSV loader: (lat, lon)
                # - DB loader: (id, latitude, longitude)
                first = batch[0]
                if isinstance(first, tuple) and len(first) == 3:
                    lats = [x[1] for x in batch]
                    lons = [x[2] for x in batch]
                else:
                    lats = [x[0] for x in batch]
                    lons = [x[1] for x in batch]

                for start, end in months:
                    data = fetch_hourly(
                        latitudes=lats,
                        longitudes=lons,
                        start_date=start,
                        end_date=end,
                        variables=OPEN_METEO_HOURLY_VARS,
                        timezone=WF_IMPORT_TIMEZONE,
                        max_retries=OPEN_METEO_MAX_RETRIES
                    )

                for idx, location_data in enumerate(data):
                    hourly = location_data["hourly"]

                    for t, temp, wind, rain in zip(
                        hourly["time"],
                        hourly["temperature_2m"],
                        hourly["wind_speed_10m"],
                        hourly["precipitation"]
                    ):
                        if export_to_csv:
                            writer.writerow([
                                lons[idx],
                                lats[idx],
                                t,
                                temp,
                                wind,
                                rain
                            ])

                        if export_to_postgres and db_adapter is not None:
                            # when DB adapter is present we expect locations to be tuples (city_id, lat, lon)
                            cid = None
                            try:
                                cid = batch[idx][0]
                            except Exception:
                                cid = None

                            db_buffer.append({
                                "city_id": cid,
                                "timestamp_utc": t,
                                "temperature_c": temp,
                                "wind_speed": wind,
                                "precipitation": rain
                            })

                pbar.update(1)
                time.sleep(OPEN_METEO_THROTTLE_SECONDS)

    if export_to_csv:
        csv_out.close()

    if export_to_postgres and db_adapter is not None and db_buffer:
        # insert in one shot using optimized method
        db_adapter.insert_wfactuals(db_buffer)

def load_locations(
        db_dsn: Optional[str], 
        cities_csv_input: Optional[str]):

    if (cities_csv_input is None or cities_csv_input.strip()  == "") and (db_dsn is None or db_dsn.strip() == ""):
        raise SystemExit("Cities CSV input or DB DSN must be provided")

    if not db_dsn.strip() == "":
        locations = load_cities_from_db(db_dsn)
    else:
        locations = load_cities_from_csv(cities_csv_input)
    return locations


def import_wf_actuals_from_csv(
        wf_actual_csv_input: str,
        db_dsn: Optional[str] = None) -> None:
    """Import weather forecast actuals from CSV file and insert into Postgres.

    The input CSV is expected to contain columns similar to:
      longitude, latitude, timestamp_utc, temperature_c, wind_speed_m_s, precipitation_mm

    This function requires a valid db_dsn because we resolve city_id by matching
    longitude/latitude against the cities table. Rows without a matching city_id
    are skipped.
    """
    if not wf_actual_csv_input or wf_actual_csv_input.strip() == "":
        raise SystemExit("No weather CSV input provided.")

    if not db_dsn or db_dsn.strip() == "":
        raise SystemExit("Importing wf_actuals into Postgres requires a valid db_dsn.")

    from adapters import WeatherForecastPgDbAdapter

    db_adapter = WeatherForecastPgDbAdapter(db_dsn)

    # Build lookup from (rounded_lon, rounded_lat) -> city_id for exact/near-exact matching
    lookup_by_lonlat = {}
    try:
        rows = db_adapter.read_all_cities()
        for r in rows:
            try:
                lon = float(r.get("longitude"))
                lat = float(r.get("latitude"))
                cid = r.get("id") or r.get("city_id")
                lookup_by_lonlat[(round(lon, 6), round(lat, 6))] = cid
            except Exception:
                continue
    except Exception:
        raise SystemExit("Failed to read cities from database. Cannot resolve city ids.")

    data_buffer = []

    # Read CSV either from Azure Blob or local path
    if wf_actual_csv_input.startswith("blob://"):
        blob_name = wf_actual_csv_input[len("blob://"):] 
        rows = _read_csv_from_blob(blob_name)
    else:
        with open(wf_actual_csv_input, newline="", encoding="utf-8") as f:
            rows = list(csv.DictReader(f))

    total_rows = len(rows)

    pbar_ctx = tqdm(total=total_rows, desc="Importing wf_actuals from CSV", unit="row") if tqdm else _NoopProgress()
    with pbar_ctx as pbar:
        for row in rows:
            lon_val = None
            lat_val = None
            for k in ("longitude", "Longitude", "lon", "Lon"):
                if k in row and row[k] not in (None, ""):
                    lon_val = row[k]; break
            for k in ("latitude", "Latitude", "lat", "Lat"):
                if k in row and row[k] not in (None, ""):
                    lat_val = row[k]; break

            if lon_val is None or lat_val is None:
                pbar.update(1); continue

            try:
                lon_f = float(lon_val); lat_f = float(lat_val)
            except Exception:
                pbar.update(1); continue

            key = (round(lon_f, 6), round(lat_f, 6))
            cid = lookup_by_lonlat.get(key)
            if cid is None:
                pbar.update(1); continue

            timestamp = row.get("timestamp_utc") or row.get("time") or row.get("timestamp")
            temp_s = row.get("temperature_c") or row.get("temperature")
            wind_s = row.get("wind_speed_m_s") or row.get("wind_speed")
            precip_s = row.get("precipitation_mm") or row.get("precipitation")

            try:
                temperature_c = float(temp_s) if temp_s not in (None, "") else None
            except Exception:
                temperature_c = None
            try:
                wind_speed = float(wind_s) if wind_s not in (None, "") else None
            except Exception:
                wind_speed = None
            try:
                precipitation = float(precip_s) if precip_s not in (None, "") else None
            except Exception:
                precipitation = None

            data_buffer.append({
                "city_id": cid,
                "timestamp_utc": timestamp,
                "temperature_c": temperature_c,
                "wind_speed": wind_speed,
                "precipitation": precipitation
            })

            if len(data_buffer) >= WF_IMPORT_CSV_INPUT_READ_BATCH_SIZE:
                db_adapter.insert_wfactuals(data_buffer)
                data_buffer = []

            pbar.update(1)

    if data_buffer:
        db_adapter.insert_wfactuals(data_buffer)

    return


def import_cities(input_path: str, dsn: str) -> int:

    if dsn.strip() == "":
        raise SystemExit("Invalid DSN")

    """Import cities CSV into the `cities` table using the DB adapter.

    Returns the number of imported rows.
    """
    # lazy imports so module import is cheap
    from adapters import WeatherForecastPgDbAdapter

    locations = load_cities_from_csv(input_path)  # list of (lon, lat, name)
    adapter = WeatherForecastPgDbAdapter(dsn)

    items = []
    for lon, lat, name in locations:
        items.append({
            "name": name,
            "longitude": lon,
            "latitude": lat
        })

    adapter.insert_cities(items)
    return len(items)