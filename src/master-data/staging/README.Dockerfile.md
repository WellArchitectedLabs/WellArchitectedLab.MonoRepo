This folder provides Dockerfiles for three separate workloads that run the master-data staging Python app.

Overview
- `Dockerfile.wf-actuals-range` - workload image for the "wf actuals range database importer". Default command runs `wf_import --export-to-postgres`; job runner must provide `--from-date` and `--to-date` as args or environment-derived args.
- `Dockerfile.wf-initializer` - workload image for the "wf actuals database initializer". Caller must mount the CSV and pass `--input /path/to/file.csv`.
- `Dockerfile.cities-initializer` - workload image for the "cities initializer". Caller must mount the cities CSV and pass `--input /path/to/cities.csv`.

Why separate images?
- Each workload is conceptually different and may have different default commands or RBAC requirements. Keeping thin, single-responsibility images reduces risk of accidental collisions when scheduled concurrently.

Build
1. From project root, change to this folder:

```bash
cd src/master-data/staging
```

2. Build each image with an explicit tag:

```bash

# wf actuals range importer
docker build -f Dockerfile.wf-actuals-range -t masterdata-wf-actuals-range:latest .

# wf initializer
docker build -f Dockerfile.wf-initializer -t masterdata-wf-initializer:latest .

# cities initializer
docker build -f Dockerfile.cities-initializer -t masterdata-cities-initializer:latest .
```

Run (local testing)
- Provide runtime args and mount a host directory as the data volume:

```bash
# wf actuals range (dates should be injected by job runner; shown here as args):
docker run --rm \
  masterdata-wf-actuals-range:latest \
  --export-to-postgres --from-date 2026-01-23 --to-date 2026-01-25

# wf initializer (mount host/data to /data in container and pass input arg):
docker run --rm -v $(pwd)/test-data:/data masterdata-wf-initializer:latest --input /data/weather_hourly.csv

# cities initializer:
docker run --rm -v $(pwd)/test-data:/data masterdata-cities-initializer:latest --input /data/cities.csv
```

Kubernetes / Job Runner guidance
- Use a separate Kubernetes Job or ArgoCD Workflow resource per workload. Each Job should:
  - Reference the appropriate image tag above.
  - Provide dates or input file path as command args (args override the image `CMD`).
  - Mount a PVC (or blobfuse/csi secret) to a path like `/data` and supply `--input /data/...`.
  - Run with a non-root user (images already switch to `appuser`).

Examples (conceptual):

- Argo CD/Workflow YAML can set `command: ["python","src/main.py"]` and `args: ["wf_import","--export-to-postgres","--from-date","$(params.from)","--to-date","$(params.to)"]`.
- For initializers, set `args: ["wf_import","--input","/data/file.csv"]` and mount the PVC at `/data`.

Safety / concurrency notes
- Images are independent; they don't use a shared working directory or system-level state. Concurrency safety depends on the database and how `main.py` behaves; prefer creating jobs that don't overlap the same DB rows/time ranges.

If you want
- I can: create example Kubernetes Job manifests for each workload that show how to mount PVCs and pass parameters.
- Or change the images to use an ENTRYPOINT script that sources dates from environment variables and validates inputs before invoking `main.py`.
