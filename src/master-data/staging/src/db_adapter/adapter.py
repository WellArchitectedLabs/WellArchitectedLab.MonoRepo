import psycopg2
import psycopg2.extras
from typing import List, Dict, Any, Iterable


class PostgresDbAdapter:
    BULK_THRESHOLD = 1000

    def __init__(self, dsn: str):
        """
        :param dsn: psycopg2 DSN string
        """
        self._dsn = dsn

    def insert(self, table: str, items: List[Dict[str, Any]]) -> None:
        """
        Public insert method.
        Chooses bulk insert automatically when item count exceeds threshold.
        """
        if not items:
            return

        if len(items) > self.BULK_THRESHOLD:
            self._bulk_insert(table, items)
        else:
            self._standard_multi_insert(table, items)

    def read_cities(self, city_names: List[str]) -> List[Dict[str, Any]]:
        """
        Optimized read for cities table.
        Uses PostgreSQL ANY operator instead of large IN clauses.
        """
        if not city_names:
            return []

        query = """
            SELECT id, city_name, country_code
            FROM cities
            WHERE city_name = ANY(%s)
        """

        with psycopg2.connect(self._dsn) as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cur:
                cur.execute(query, (city_names,))
                return cur.fetchall()

    # ==========================
    # Private helpers
    # ==========================

    def _standard_multi_insert(self, table: str, items: List[Dict[str, Any]]) -> None:
        """
        Standard multi-row insert using executemany.
        Suitable for smaller datasets.
        """
        columns = items[0].keys()
        column_list = ", ".join(columns)
        placeholders = ", ".join([f"%({c})s" for c in columns])

        query = f"""
            INSERT INTO {table} ({column_list})
            VALUES ({placeholders})
        """

        with psycopg2.connect(self._dsn) as conn:
            with conn.cursor() as cur:
                cur.executemany(query, items)

    def _bulk_insert(self, table: str, items: List[Dict[str, Any]]) -> None:
        """
        High-performance bulk insert using psycopg2.execute_values.
        This is the closest equivalent to SQL Server BulkInsert / TVPs.
        """
        columns = list(items[0].keys())
        column_list = ", ".join(columns)

        values: Iterable[tuple] = (
            tuple(item[col] for col in columns)
            for item in items
        )

        query = f"""
            INSERT INTO {table} ({column_list})
            VALUES %s
        """

        with psycopg2.connect(self._dsn) as conn:
            with conn.cursor() as cur:
                psycopg2.extras.execute_values(
                    cur,
                    query,
                    values,
                    page_size=1000
                )
