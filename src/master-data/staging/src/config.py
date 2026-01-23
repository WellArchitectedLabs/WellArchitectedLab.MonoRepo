# src/config.py

API_URL = "https://archive-api.open-meteo.com/v1/archive"

HOURLY_VARS = [
    "temperature_2m",
    "wind_speed_10m",
    "precipitation"
]

BATCH_SIZE = 10
MAX_RETRIES = 5

TIMEZONE = "UTC"

THROTTLE_SECONDS = 2

RETRY_DELAY = 5

OUTPUT_CSV = "output2/weather_hourly.csv"

POSTGRES_DSN = "postgresql://postgres:postgres@localhost:5432/master_data?sslmode=disable"