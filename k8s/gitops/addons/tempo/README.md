It is important to understand the difference between:
- Metrices (time series data)
- Traces (span based graph data)
- Logs (Append only data)

Comparaison table:

| Property       | Metrics                | Logs               | Traces                                 |
|----------------|------------------------|--------------------|----------------------------------------|
| Data shape     | Numeric time series    | Append-only text   | Directed acyclic graph (DAG)           |
| Cardinality    | Low                    | High               | Extremely high                         |
| Access pattern | Aggregate over time    | Search             | Reconstruct request paths              |
| Storage        | Indexed time series    | Indexed text       | Span graph                             |
| Example        | CPU %, request rate    | Error stack trace  | One request across 12 services         |


Mental model: Push based model using OTLP:


               ┌────────────────────┐
               │  Applications       │
               │  (OTel SDKs)        │
               └─────────┬──────────┘
                         │
            ┌────────────┴────────────┐
            │ OpenTelemetry Collector  │
            │ (traces, optional logs) │
            └────────────┬────────────┘
                         │
                   ┌─────▼─────┐
                   │  Tempo    │
                   └───────────┘

Mental model: Pull based model for metrics:

   ┌──────────────────────────────────────┐
   │ Prometheus (pull)                     │
   │ - node_exporter                       │
   │ - kube-state-metrics                  │
   │ - Istio / Envoy metrics               │
   └──────────────────────────────────────┘

Metrics are pull based:
- For safety
- Preventing DDoS due to resource restarts
- Counter intuitive with metrics that are not pushed by resources.

Semantically: metrics, traces and logs are all called telemetry as per the Open Telemtry Protocol (OTLP).

Traces flow from istio side cars.
To OTLP servers.
To Tempo database.
To Grafana.

Request enters the mesh
        ↓
Istio sidecar (Envoy) creates spans
        ↓   (OTLP, push)
OpenTelemetry Collector
        ↓   (OTLP, exports)
Tempo
        ↓   (query by trace ID)
Grafana

Tempo is a storage which is optimized for queries.
Tempo dies not export metrics.
We configure the open telemtry collector to point to the tempo database using a helm value.

TODO: make a diagram using mermaid.

