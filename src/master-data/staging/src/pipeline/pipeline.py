"""High-level pipeline orchestrator.

Provides two functions used by the CLI in `main.py`:
- cities_import(input_path, dsn)
- fetch(from_date, to_date, dsn, export_to_csv, export_to_postgres)

This keeps CLI small and centralises logic here for testability.
"""
from typing import Optional
from datetime import date


def cities_import(input_path: str, dsn: str) -> int:
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


def wf_imports(from_date: date, to_date: date, dsn: Optional[str] = None, *, export_to_csv: bool = False, export_to_postgres: bool = True) -> None:
    """Run the hourly fetch pipeline.

    - If export_to_postgres is True, `dsn` is required and locations are loaded from DB.
    - Otherwise the behaviour falls back to the extractor which may require an input CSV.
    """
    from . import extractor

    db = None
    if export_to_postgres:
        if not dsn:
            raise SystemExit("--dsn is required when --export-to-postgres is set")
        from db_adapter.adapter import PostgresDbAdapter
        db = PostgresDbAdapter(dsn)

    # extractor.run will load locations from DB when db_adapter is provided
    extractor.run(
        from_date=from_date,
        to_date=to_date,
        db_adapter=db,
        export_to_csv=export_to_csv,
        export_to_postgres=export_to_postgres
    )
