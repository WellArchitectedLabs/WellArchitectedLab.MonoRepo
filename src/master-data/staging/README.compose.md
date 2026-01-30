# Docker Compose – Staging Service

## Overview

This Docker Compose setup defines the **staging data pipeline** for the master data domain. It is composed of:

1. A PostgreSQL database
2. A Flyway migration step
3. One-time data import jobs

The import jobs are **batch-style containers** that:

* Run once
* Perform their task
* Exit

They are **not long-running services**.

---

## Services

### `masterdata-postgres`

PostgreSQL 15 database used by all import jobs.

* Persistent storage via a named Docker volume
* Healthcheck enabled (`pg_isready`)

This service must be running and healthy before any other service can execute.

---

### `migrate`

Flyway-based database migration service.

* Runs automatically once PostgreSQL is healthy
* Applies all SQL migrations from `./data/migrations`
* Exits after completion

This service ensures the database schema is up to date before any data import occurs.

---

### `cities-import`

Initial data import service for cities.

* Imports cities from `capitals.csv`
* **Prerequisite for all weather actuals imports**
* Runs once and exits

This service **must be executed before**:

* `wf-actuals-import`
* `wf-actuals-range-db-import`

#### Command

```bash
docker compose run --rm --build cities-import
```

---

### `wf-actuals-import`

Bulk import of historical weather actuals from CSV.

* Depends on PostgreSQL
* Assumes cities have already been imported
* Runs once and exits

#### Command

```bash
docker compose run --rm --build wf-actuals-import
```

---

### `wf-actuals-range-db-import`

On-demand weather actuals import for a specific date range.

* Intended for **manual execution only**
* Imports data directly from an external source into the database
* Requires cities to already exist

#### Command

```bash
FROM_DATE=2026-01-01 TO_DATE=2026-01-02 \
docker compose --profile manual run --rm wf-actuals-range-db-import
```

If no environment variables are provided, default dates defined in `docker-compose.yml` are used.

---

## Execution order

The expected setup flow is:

1. Start PostgreSQL
2. Apply database migrations
3. Import cities
4. Import weather actuals
5. Import weather actuals for a date range (manual, on-demand)

Only step **3** is strictly required before weather imports.

---

## Notes

* All import services use `restart: "no"` to ensure they only run once
* No import service exposes ports
* All orchestration dependencies are handled via health checks and completion conditions

This setup is designed to be deterministic, repeatable, and safe to run locally or in CI.
