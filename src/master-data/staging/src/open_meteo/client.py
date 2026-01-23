import time
import requests
from config import API_URL, RETRY_DELAY, MAX_RETRIES

def fetch_hourly(
    latitudes,
    longitudes,
    start_date,
    end_date,
    variables,
    timezone,
    max_retries
):
    params = {
        "latitude": ",".join(map(str, latitudes)),
        "longitude": ",".join(map(str, longitudes)),
        "start_date": start_date.isoformat(),
        "end_date": end_date.isoformat(),
        "hourly": ",".join(variables),
        "timezone": timezone
    }

    for attempt in range(1, max_retries + 1):
        try:
            if attempt > 1:
                print(f"[INFO] Retrying request for {len(latitudes)} locations from {start_date} to {end_date}, attempt {attempt}")
            response = requests.get(API_URL, params=params, timeout=60)
            if response.status_code == 429:
                print(f"[WARN] 429 Too Many Requests. Backing off for {RETRY_DELAY} seconds")
                time.sleep(RETRY_DELAY * attempt)
                continue
            elif response.status_code >= 500:
                print(f"[WARN] Server error {response.status_code}. Retrying in {RETRY_DELAY * attempt} seconds")
                time.sleep(RETRY_DELAY * attempt)
                continue
            response.raise_for_status()
            return response.json()
        except requests.RequestException as e:
            print(f"[ERROR] Request failed: {e}. Retry {attempt}/{MAX_RETRIES}")
            time.sleep(RETRY_DELAY * attempt)
    raise RuntimeError(f"Failed to fetch data after {MAX_RETRIES} attempts for {start} to {end}")
