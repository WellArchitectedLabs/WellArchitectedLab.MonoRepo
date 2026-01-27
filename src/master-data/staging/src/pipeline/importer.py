"""
This implementation compares development experience between:
- Accessing CSV files via Kubernetes CSI-mounted volumes (local filesystem)
- No usage of Azure Sdk. Implementation is relying on CSI driver for blob storage connection

"""

import csv
import os
import time
from datetime import date, timedelta
from typing import Optional
from dateutil.relativedelta import relativedelta
from tqdm import tqdm

from config import (
    WF_IMPORT_CSV_INPUT_READ_BATCH_SIZE,
    WF_IMPORT_OUTPUT_CSV,
    OPEN_METEO_BATCH_SIZE,
    OPEN_METEO_THROTTLE_SECONDS,
    WF_IMPORT_TIMEZONE,
    OPEN_METEO_MAX_RETRIES,
    OPEN_METEO_HOURLY_VARS,
)

from open_meteo import fetch_hourly


class _NoopProgress:
    def __init__(self, *a, **kw):
        pass

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc, tb):
        return False

    def update(self, n=1):
        return


# -----------------------------
# CSV LOADERS (LOCAL FILESYSTEM)
# -----------------------------

def load_cities_from_csv(path: str):
    """
    Load cities from a local CSV file.

    The file must be provided via a CSI-mounted volume.
    """
    if not path or path.strip() == "":
        raise SystemExit("Cities CSV path is empty")

    if not os.path.exists(path):
        raise SystemExit(f"Cities CSV file not found: {path}")

    locations = []
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
    from adapters import WeatherForecastPgDbAdapter
    db_adapter = WeatherForecastPgDbAdapter(db_dsn)
    rows = db_adapter.read_all_cities()
    return [(r["id"], r["latitude"], r["longitude"]) for r in rows]


# -----------------------------
# HELPERS
# -----------------------------

def chunked(seq, size):
    for i in range(0, len(seq), size):
        yield seq[i:i + size]


def month_ranges_between(start, end):
    ranges = []
    current = start.replace(day=1)

    while current <= end:
        month_end = (current + relativedelta(months=1)) - timedelta(days=1)
        ranges.append((max(start, current), min(end, month_end)))
        current += relativedelta(months=1)

    return ranges


def load_locations(db_dsn: Optional[str], cities_csv_input: Optional[str]):
    if (not cities_csv_input or cities_csv_input.strip() == "") and (not db_dsn or db_dsn.strip() == ""):
        raise SystemExit("Cities CSV input or DB DSN must be provided")

    if db_dsn and db_dsn.strip() != "":
        return load_cities_from_db(db_dsn)

    return load_cities_from_csv(cities_csv_input)


# -----------------------------
# MAIN IMPORT LOGIC
# -----------------------------

def import_wf_actuals_from_open_meteo(
    from_date: date,
    to_date: date,
    db_dsn: str,
    cities_csv_input: Optional[str] = None,
    export_to_csv: bool = False,
    export_to_postgres: bool = True
) -> None:

    if from_date >= to_date:
        raise SystemError("from_date must be strictly before to_date")

    if export_to_postgres and not db_dsn.strip():
        raise SystemError("Postgres export requires a valid db_dsn")

    locations = load_locations(db_dsn, cities_csv_input)
    months = month_ranges_between(from_date, to_date)

    total_requests = (
        ((len(locations) + OPEN_METEO_BATCH_SIZE - 1) // OPEN_METEO_BATCH_SIZE)
        * len(months)
    )

    db_adapter = None
    if db_dsn.strip():
        from adapters import WeatherForecastPgDbAdapter
        db_adapter = WeatherForecastPgDbAdapter(db_dsn)

    csv_out = None
    writer = None
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

    db_buffer = []

    pbar_ctx = tqdm(total=total_requests, desc="Fetching Open-Meteo data", unit="request") if tqdm else _NoopProgress()
    with pbar_ctx as pbar:
        for batch in chunked(locations, OPEN_METEO_BATCH_SIZE):
            first = batch[0]
            if len(first) == 3:
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
                            writer.writerow([lons[idx], lats[idx], t, temp, wind, rain])

                        if export_to_postgres and db_adapter:
                            cid = batch[idx][0] if len(batch[idx]) == 3 else None
                            db_buffer.append({
                                "city_id": cid,
                                "timestamp_utc": t,
                                "temperature_c": temp,
                                "wind_speed": wind,
                                "precipitation": rain
                            })

                pbar.update(1)
                time.sleep(OPEN_METEO_THROTTLE_SECONDS)

    if csv_out:
        csv_out.close()

    if export_to_postgres and db_adapter and db_buffer:
        db_adapter.insert_wfactuals(db_buffer)


# -----------------------------
# CSV IMPORT TO POSTGRES
# -----------------------------

def import_wf_actuals_from_csv(
    wf_actual_csv_input: str,
    db_dsn: Optional[str] = None
) -> None:

    if not wf_actual_csv_input or wf_actual_csv_input.strip() == "":
        raise SystemExit("No weather CSV input provided")

    if not os.path.exists(wf_actual_csv_input):
        raise SystemExit(f"CSV file not found: {wf_actual_csv_input}")

    if not db_dsn or db_dsn.strip() == "":
        raise SystemExit("Postgres import requires a valid db_dsn")

    from adapters import WeatherForecastPgDbAdapter
    db_adapter = WeatherForecastPgDbAdapter(db_dsn)

    lookup_by_lonlat = {}
    for r in db_adapter.read_all_cities():
        lookup_by_lonlat[(round(float(r["longitude"]), 6), round(float(r["latitude"]), 6))] = r["id"]

    with open(wf_actual_csv_input, newline="", encoding="utf-8") as f:
        rows = list(csv.DictReader(f))

    data_buffer = []
    pbar_ctx = tqdm(total=len(rows), desc="Importing wf_actuals from CSV", unit="row") if tqdm else _NoopProgress()

    with pbar_ctx as pbar:
        for row in rows:
            try:
                lon = round(float(row["longitude"]), 6)
                lat = round(float(row["latitude"]), 6)
            except Exception:
                pbar.update(1)
                continue

            cid = lookup_by_lonlat.get((lon, lat))
            if not cid:
                pbar.update(1)
                continue

            data_buffer.append({
                "city_id": cid,
                "timestamp_utc": row["timestamp_utc"],
                "temperature_c": float(row["temperature_c"]),
                "wind_speed": float(row["wind_speed_m_s"]),
                "precipitation": float(row["precipitation_mm"])
            })

            if len(data_buffer) >= WF_IMPORT_CSV_INPUT_READ_BATCH_SIZE:
                db_adapter.insert_wfactuals(data_buffer)
                data_buffer = []

            pbar.update(1)

    if data_buffer:
        db_adapter.insert_wfactuals(data_buffer)


# -----------------------------
# CITIES IMPORT
# -----------------------------

def import_cities(input_path: str, dsn: str) -> int:
    if not os.path.exists(input_path):
        raise SystemExit(f"Cities CSV file not found: {input_path}")
    """Import cities CSV into the `cities` table using the DB adapter.

    Returns the number of imported rows.
    """
    # lazy imports so module import is cheap    from adapters import WeatherForecastPgDbAdapter
    adapter = WeatherForecastPgDbAdapter(dsn)

    locations = load_cities_from_csv(input_path)

    items = [{
        "name": name,
        "longitude": lon,
        "latitude": lat
    } for lon, lat, name in locations]

    adapter.insert_cities(items)
    return len(items)