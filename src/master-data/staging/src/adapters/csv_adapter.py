import csv
from typing import List, Tuple

def load_cities_from_csv(path: str) -> List[Tuple[float, float, str]]:
    locations = []
    with open(path, newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        for row in reader:
            locations.append((
                float(row["Longitude"]),
                float(row["Latitude"]),
                row.get("Name", "")
            ))
    return locations