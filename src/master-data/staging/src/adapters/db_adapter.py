import psycopg2
import psycopg2.extras
from psycopg2 import sql
from typing import List, Dict, Any, Iterable


class WeatherForecastPgDbAdapter:
    BULK_THRESHOLD = 1000

    def __init__(self, dsn: str):
        """
        :param dsn: psycopg2 DSN string
        """
        self._dsn = dsn


    def read_all_cities(self) -> List[Dict[str, Any]]:
        """Return all cities as list of dicts with latitude/longitude and id."""
        query = "SELECT id, name, longitude, latitude FROM cities"
        with psycopg2.connect(self._dsn) as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cur:
                cur.execute(query)
                return cur.fetchall()

    def insert_cities(self, items: List[Dict[str, Any]]) -> None:
        """Insert city records. Items are dicts with keys: name, longitude, latitude"""
        if not items:
            return
        columns = ["name", "longitude", "latitude"]
        values = [tuple(item[c] for c in columns) for item in items]
        query = f"INSERT INTO cities ({', '.join(columns)}) VALUES %s"

        with psycopg2.connect(self._dsn) as conn:
            with conn.cursor() as cur:
                psycopg2.extras.execute_values(cur, query, values, page_size=1000)

    def insert_wfactuals(self, items: List[Dict[str, Any]]) -> None:
        """Insert wfactuals records. Items should match wfactuals columns."""
        if not items:
            return
        
        columns = list(items[0].keys())
        column_list = ", ".join(columns)
        values = (tuple(item[col] for col in columns) for item in items)
        query = f"INSERT INTO wf_actuals ({column_list}) VALUES %s"

        with psycopg2.connect(self._dsn) as conn:
            with conn.cursor() as cur:
                psycopg2.extras.execute_values(cur, query, values, page_size=1000)
