import argparse
from datetime import date
from pipeline.pipeline import cities_import as pipeline_cities_import
from pipeline.pipeline import wf_import as pipeline_wf_import
from config import POSTGRES_DSN


def main():
    p = argparse.ArgumentParser(prog="master-data")
    sub = p.add_subparsers(dest="cmd", required=True)

    # cities import entrypoint
    cities_import = sub.add_parser("cities_import", help="Import cities CSV into cities table")
    cities_import.add_argument("--input", required=True, help="Path to cities csv")

    # wf imports entrypoint
    wf_import = sub.add_parser("wf_import", help="Import hourly weather data for cities from the provided date range. Supports up to 2 years of historical data for all world cities.")
    wf_import.add_argument("--from-date", required=False)
    wf_import.add_argument("--to-date", required=False)
    wf_import.add_argument("--export-to-csv", action="store_true", default=False)
    wf_import.add_argument("--export-to-postgres", action="store_true", default=False)
    wf_import.add_argument("--cities-input", required=False, help="Path to cities csv")
    wf_import.add_argument("--input", required=False, help="Path to weather csv. Imports the given csv file into the configured database on config level. Use when you would like to import an already generated weatehr forecasts file.")

    args = p.parse_args()

    if args.cmd == "cities_import":
        count = pipeline_cities_import(args.input, dsn=POSTGRES_DSN)
        print(f"Imported {count} cities into DB")

    elif args.cmd == "wf_import":
        # safely parse optional date arguments
        from_date_val = None
        to_date_val = None
        if getattr(args, "from_date", None):
            s = args.from_date
            if isinstance(s, str) and s.strip() != "":
                from_date_val = date.fromisoformat(s)
        if getattr(args, "to_date", None):
            s = args.to_date
            if isinstance(s, str) and s.strip() != "":
                to_date_val = date.fromisoformat(s)
        pipeline_wf_import(
            from_date=from_date_val,
            to_date=to_date_val,
            dsn=POSTGRES_DSN,
            cities_csv_input=args.cities_input,
            export_to_csv=args.export_to_csv,
            export_to_postgres=args.export_to_postgres,
            weather_csv_input=args.input
        )


if __name__ == "__main__":
    main()
