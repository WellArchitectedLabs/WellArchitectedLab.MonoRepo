"""High-level pipeline orchestrator.

Provides two functions used by the CLI in `main.py`:
- cities_import(input_path, dsn)
- fetch(from_date, to_date, dsn, export_to_csv, export_to_postgres)

This keeps CLI small and centralises logic here for testability.
"""
from typing import Optional
from datetime import date


def cities_import(input_path: str, dsn: str) -> int:
    """Run the hourly fetch pipeline.

    - If export_to_postgres is True, `dsn` is required and locations are loaded from DB.
    - Otherwise the behaviour falls back to the extractor which may require an input CSV.
    """
    from . import importer

    # importer.run will load locations from DB when db_adapter is provided
    return importer.import_cities(
        input_path=input_path,
        dsn=dsn
    )


def wf_imports(
        from_date: Optional[date], 
        to_date: Optional[date], 
        dsn: Optional[str] = None, 
        cities_csv_input: Optional[str] = None, 
        export_to_csv: bool = False, 
        export_to_postgres: bool = True,
        weather_csv_input: Optional[str] = None) -> None:
    """Run the hourly fetch pipeline.

    - If export_to_postgres is True, `dsn` is required and locations are loaded from DB.
    - Otherwise the behaviour falls back to the extractor which may require an input CSV.
    """
    from . import importer

    if(weather_csv_input is not None and weather_csv_input.strip() != ""):
        importer.import_wf_actuals_from_csv(
            wf_actual_csv_input=weather_csv_input,
            db_dsn=dsn
        )
        return
    # importer.import_wf_actuals will load locations from DB
    # Contacts open meteo API for collecting weather actuals then
    # Then store them into database
    importer.import_wf_actuals_from_open_meteo(
        from_date=from_date,
        to_date=to_date,
        db_dsn=dsn,
        cities_csv_input=cities_csv_input,
        export_to_csv=export_to_csv,
        export_to_postgres=export_to_postgres
    )
