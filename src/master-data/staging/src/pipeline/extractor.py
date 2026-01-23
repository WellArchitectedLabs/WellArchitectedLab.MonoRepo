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

def load_locations_from_db():
    # Implement database loading logic here
    pass

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

def run(from_date, to_date, input_csv):

    locations = load_locations_from_csv(input_csv)
    months = month_ranges_between(from_date, to_date)

    total_requests = (
        ((len(locations) + BATCH_SIZE - 1) // BATCH_SIZE)
        * len(months)
    )


    with open(OUTPUT_CSV, "w", newline="", encoding="utf-8") as out:
        writer = csv.writer(out)
        writer.writerow([
            "longitude",
            "latitude",
            "timestamp_utc",
            "temperature_c",
            "wind_speed_m_s",
            "precipitation_mm"
        ])

        with tqdm(
            total=total_requests,
            desc="Fetching Open-Meteo data",
            unit="request"
        ) as pbar:

            for batch in chunked(locations, BATCH_SIZE):
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
                            writer.writerow([
                                lons[idx],
                                lats[idx],
                                t,
                                temp,
                                wind,
                                rain
                            ])

                    pbar.update(1)
                    time.sleep(THROTTLE_SECONDS)
