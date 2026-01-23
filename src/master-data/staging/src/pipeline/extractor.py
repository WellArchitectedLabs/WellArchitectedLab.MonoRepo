import csv
import time
from datetime import timedelta
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


def _write_row_to_db(lon, lat, t, temp, wind, rain):
    return {
        "longitude": lon,
        "latitude": lat,
        "timestamp_utc": t,
        "temperature_c": temp,
        "wind_speed": wind,
        "precipitation": rain
    }

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

def run(from_date, to_date, input_csv=None, db_adapter=None, export_to_csv=True, export_to_postgres=False):

    if db_adapter is not None:
        locations = load_locations_from_db(db_adapter)
    else:
        if not input_csv:
            raise SystemExit("--input is required when no db adapter is provided")
        locations = load_locations_from_csv(input_csv)
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
