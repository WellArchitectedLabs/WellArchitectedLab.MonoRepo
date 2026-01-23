import argparse
from datetime import date
from pipeline.extractor import run

def parse_args():
    p = argparse.ArgumentParser()
    p.add_argument("--input", required=True)
    p.add_argument("--from-date", required=True)
    p.add_argument("--to-date", required=True)
    return p.parse_args()

if __name__ == "__main__":
    args = parse_args()

    run(
        input_csv=args.input,
        from_date=date.fromisoformat(args.from_date),
        to_date=date.fromisoformat(args.to_date)
    )
