OTLP protocol needs is based on a exporter / receiver termonilogy:

- Istio sidecar *emit* traces via OTLP protocol to the open telemtry collector.
- The telemetry Collector receives the traces via a *receiver* component (needs to be explicitly enabled in yaml).
- Exporter processes data using a *processor* component (Please check yaml for processir configuration).
- A pipeline is defined in order to specify the processor sequence (reception, batch, export). This is called a *service* in OTLP terminology.
- Open Telemetry Collector *exports* data to Tempo via the telemtry *exporter*.
- Tempo has only a *reveiver* component.

TODO: clean architecture pipelines for:
    - How metrics flow and are detected using pod monitor to prometheus operator to the prometheus database
    - How traces flow from envoy sidecars (who natively generate traces) to open telemetry collector (which defines a service of receiver, processors and exporters) to which receives the traces via endpoint Tempo to Grafana which runs queries and visualizes traces.