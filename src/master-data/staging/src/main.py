import argparse
from datetime import date
from pipeline.pipeline import cities_import as pipeline_cities_import
from pipeline.pipeline import wf_imports as pipeline_wf_imports
from config import POSTGRES_DSN


def main():
    p = argparse.ArgumentParser(prog="master-data")
    sub = p.add_subparsers(dest="cmd", required=True)

    # cities import entrypoint
    imp = sub.add_parser("cities_import", help="Import capitals CSV into cities table")
    imp.add_argument("--input", required=True, help="Path to capitals csv")

    # fetch entrypoint
    fetch = sub.add_parser("wf_imports", help="Import hourly weather data for cities from the provided date range. Supports up to 2 years of historical data for all world capitals.")
    fetch.add_argument("--from-date", required=True)
    fetch.add_argument("--to-date", required=True)
    fetch.add_argument("--export-to-csv", action="store_true", default=False)
    fetch.add_argument("--export-to-postgres", action="store_true", default=False)

    args = p.parse_args()

    if args.cmd == "cities_import":
        count = pipeline_cities_import(args.input, dsn=POSTGRES_DSN)
        print(f"Imported {count} cities into DB")

    elif args.cmd == "wf_imports":
        pipeline_wf_imports(
            from_date=date.fromisoformat(args.from_date),
            to_date=date.fromisoformat(args.to_date),
            dsn=POSTGRES_DSN,
            export_to_csv=args.export_to_csv,
            export_to_postgres=args.export_to_postgres
        )


if __name__ == "__main__":
    main()
