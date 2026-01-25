## OPEN METEO CONFIGS

OPEN_METEO_API_URL = "https://archive-api.open-meteo.com/v1/archive"

OPEN_METEO_HOURLY_VARS = [
    "temperature_2m",
    "wind_speed_10m",
    "precipitation"
]
OPEN_METEO_BATCH_SIZE = 10
OPEN_METEO_MAX_RETRIES = 5
OPEN_METEO_THROTTLE_SECONDS = 2
OPEN_METEO_RETRY_DELAY = 5

## WF IMPORTS JOB CONFIGS

WF_IMPORT_OUTPUT_CSV = "output/weather_hourly-23-to-25.csv"
WF_IMPORT_TIMEZONE = "UTC"
WF_IMPORT_CSV_INPUT_READ_BATCH_SIZE = 10000

## DATABASE CONFIG

POSTGRES_DSN = "postgresql://postgres:postgres@localhost:5432/master_data?sslmode=disable"