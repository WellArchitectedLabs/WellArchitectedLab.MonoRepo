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
import os

# Allow overriding the DSN via environment for containers / k8s jobs.
# Default kept for local dev convenience but containers should set POSTGRES_DSN.
POSTGRES_DSN = os.environ.get(
    "POSTGRES_DSN",
    "postgresql://postgres:postgres@localhost:5432/master_data?sslmode=disable",
)

## AZURE CONFIGS

## DATABASE CONFIG
import os

# Allow overriding the DSN via environment for containers / k8s jobs.
# Default kept for local dev convenience but containers should set POSTGRES_DSN.
AZ_BLOB_CONNECTION_STRING = os.environ.get(
    "AZ_BLOB_CONNECTION_STRING",
    # This is not a security leak, this is the default connection string for azurite created storages
    # Codeql analysis will be overriden in github by consequence
    "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;",
)

# Allow overriding the DSN via environment for containers / k8s jobs.
# Default kept for local dev convenience but containers should set POSTGRES_DSN.
AZ_BLOB_CONTAINER = "imports"