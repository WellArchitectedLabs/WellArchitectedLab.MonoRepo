┌─────────────────────────────────────────────────────┐
│              ARCHITECTURE LOKI                      │
├─────────────────────────────────────────────────────┤
│                                                     │
│  Promtail (DaemonSet sur chaque node)              │
│    │                                                │
│    │ 1. Découvre pods (kubernetes_sd)              │
│    │ 2. Lit logs (/var/log/pods/...)              │
│    │ 3. Parse JSON                                 │
│    │ 4. Extrait labels                             │
│    │                                                │
│    ▼                                                │
│  Gateway (NGINX)                                    │
│    │                                                │
│    │ - Point d'entrée unique                       │
│    │ - Rate limiting                                │
│    │ - Auth (si activée)                           │
│    │                                                │
│    ▼                                                │
│  Loki SingleBinary                                  │
│    │                                                │
│    ├─► Ingestion (reçoit logs)                    │
│    ├─► Indexation (TSDB index_*)                  │
│    ├─► Storage (filesystem chunks)                │
│    ├─► Query (répond à Grafana)                   │
│    └─► Compactor (supprime vieux logs)            │
│                                                     │
│  Storage (PVC 10Gi)                                │
│    ├─ index_2026-01-19/                           │
│    ├─ index_2026-01-18/                           │
│    └─ chunks/   


Loki Architecture above.
Loki exposes a single gateway for both Grafana and Promtail.
Promtail uses HTTP, Grafana uses GRPC but both point to gateway which manages both these protocols.
Promtail is a deamon set which runs alongside every pod and exports pod logs under different phases:
- Phase 1 - Pod discovery: Different jobs are available, we can use the kubernetes-pod job.
- Phase 2 - Reading logs (they are generally json formatted logs)
- Phase 3 - Parsing Json logs (map values from the json to promtail specific variables).
- Phase 5 - Variables promotion: Extracted data are variables that can be promoted to labels which makes them included into indexes (please refer to loki section to understand how loki stores indxes and chunks)
- Phase 6 - Extracting labels from temporary pod job scraper (temporary variables are prefixed with __ like metadata) using relabeling.

Loki is exposed to protail and Grafana via a unified gateway.
Loki's responsability is storing logs in a performant manner.
We can deploy Loki in different ways (microservices architecture using a CQRS approach (a read and a write service) or single binary (monolith) architecture).
Monolith architecture is usually enough.


Loki logs exeuction process:

QUERY: {pod="bff", level="error"} (last 5 min)
         │
         ▼
┌────────────────────────────────────────┐
│   INDEX TSDB                           │
├────────────────────────────────────────┤
│                                        │
│  Recherche par:                        │
│  1. Labels: pod="bff" + level="error" │
│  2. Time: 14:20 → 14:25               │
│                                        │
│  Résultat:                             │
│  ├─ 14:23:15 → chunk-abc              │
│  └─ 14:25:00 → chunk-def              │
│                                        │
└────────┬───────────────────────────────┘
         │
         │ "Va lire chunk-abc et chunk-def"
         ▼
┌────────────────────────────────────────┐
│   CHUNKS (filesystem)                  │
├────────────────────────────────────────┤
│                                        │
│  chunk-abc:                            │
│  14:23:15 ERROR Connection failed      │
│  14:23:16 INFO Retrying...            │
│  14:23:45 INFO Connected successfully  │
│                                        │
│  chunk-def:                            │
│  14:25:00 ERROR Timeout after 30s      │
│  14:25:10 WARN Falling back to cache   │
│                                        │
└────────┬───────────────────────────────┘
         │
         │ Filtre seulement level="error"
         ▼
┌────────────────────────────────────────┐
│   RÉSULTAT FINAL                       │
├────────────────────────────────────────┤
│  14:23:15 ERROR Connection failed      │
│  14:25:00 ERROR Timeout after 30s      │
└────────────────────────────────────────┘

Logs are stored into chuncks which are in file system, or for production systems in stroage accounts
An Index of type TSDB is created with: Timestamp, labels and references to chunks.
Labels contain important metadata for the log like log level (parsed by promtail and sent to loki) and pod name.

Here is what an Loki storage would look liek with index and the chunk reference from index:

┌────────────────────────────────────
│     LOKI STORAGE                   │
├────────────────────────────────────┤
│                                    │
│  INDEX (TSDB format)               │
│  ├─ index_2026-01-19/              │
│  │   ├─ pod-123 → chunk-abc        │
│  │   ├─ pod-456 → chunk-def        │
│  │   └─ timestamps, labels...      │
│                                    │
│  CHUNKS (filesystem or storage)    │
│  ├─ chunks/abc/                    │
│  │   └─ "Error connecting to DB"   │
│  ├─ chunks/def/                    │
│  │   └─ "Request completed 200"    │
│                                    │
└────────────────────────────────────┘

Tabular version:

Timestamp Labels Chunk Reference 
2026-01-19 14:23:15 pod="bff", level="error" chunk-abc
2026-01-19 14:23:16 pod="bff", level="info"chunk-abc 
2026-01-19 14:25:00 pod="bff", level="error"chunk-def
2026-01-19 15:00:00 pod="traffic", level="info" chunk-xyz

Loki needs to keep timestamp, labels (pod name and level) and surely timestap in index because it is important for fast querying (do not want to filter the chunks).