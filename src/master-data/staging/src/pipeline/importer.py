import csv
import time
from datetime import date, timedelta
from typing import Optional
from dateutil.relativedelta import relativedelta
from tqdm import tqdm

from open_meteo import fetch_hourly
from config import OUTPUT_CSV
from config import BATCH_SIZE
from config import THROTTLE_SECONDS
from config import TIMEZONE
from config import MAX_RETRIES
from config import HOURLY_VARS 


class _NoopProgress:
    def __init__(self, *a, **kw):
        pass

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc, tb):
        return False

    def update(self, n=1):
        return

def load_locations_from_csv(path):
    locations = []
    with open(path, newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        for row in reader:
            locations.append((
                float(row["Latitude"]),
                float(row["Longitude"])
            ))
    return locations

def load_locations_from_db(db_adapter):
    # Expect db_adapter.read_cities to return list of dicts with latitude/longitude
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

def import_wf_actuals(from_date: date, to_date: date, db_dsn: str, cities_csv_input: Optional[str] = None, export_to_csv: bool = False, export_to_postgres: bool = True) -> None:
    from db_adapter.adapter import PostgresDbAdapter
    db_adapter = None

    if not db_dsn.strip() == "":
        db_adapter = PostgresDbAdapter(db_dsn)
        locations = load_locations_from_db(db_adapter)
    else:
        if cities_csv_input.strip() == "" or cities_csv_input is None:
            raise SystemExit("cities --input is required when no db adapter is provided")
        locations = load_locations_from_csv(cities_csv_input)
    months = month_ranges_between(from_date, to_date)

    total_requests = (
        ((len(locations) + BATCH_SIZE - 1) // BATCH_SIZE)
        * len(months)
    )


    # prepare CSV writer if requested
    csv_out = None
    if export_to_csv:
        csv_out = open(OUTPUT_CSV, "w", newline="", encoding="utf-8")
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

            for batch in chunked(locations, BATCH_SIZE):
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
                        variables=HOURLY_VARS,
                        timezone=TIMEZONE,
                        max_retries=MAX_RETRIES
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
                time.sleep(THROTTLE_SECONDS)

    if export_to_csv:
        csv_out.close()

    if export_to_postgres and db_adapter is not None and db_buffer:
        # insert in one shot using optimized method
        db_adapter.insert_wfactuals(db_buffer)

def import_cities(input_path: str, dsn: str) -> int:

    if dsn.strip() == "":
        raise ValueError("Invalid DSN")

    """Import capitals CSV into the `cities` table using the DB adapter.

    Returns the number of imported rows.
    """
    # lazy imports so module import is cheap
    from datasource.csv_reader import load_locations
    from db_adapter.adapter import PostgresDbAdapter

    locations = load_locations(input_path)  # list of (lon, lat, name)
    adapter = PostgresDbAdapter(dsn)

    items = []
    for lon, lat, name in locations:
        items.append({
            "name": name,
            "longitude": lon,
            "latitude": lat
        })

    adapter.insert_cities(items)
    return len(items)