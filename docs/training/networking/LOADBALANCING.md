╔══════════════════════════════════════════════════════════════════════════════╗
║              HOW KUBE-PROXY SELECTS A POD (LOAD BALANCING)                 ║
╚══════════════════════════════════════════════════════════════════════════════╝

 ┌─────────────────────────────────────────────────────────────────────────┐
 │                     KUBERNETES API SERVER                               │
 │                                                                         │
 │  Service: master-data-svc                                               │
 │  ┌─────────────────────────────────────────────────────────────┐        │
 │  │ spec:                                                       │        │
 │  │   selector:                                                 │        │
 │  │     app: master-data        ◄── label selector              │        │
 │  │   ports:                                                    │        │
 │  │     - port: 8080                                            │        │
 │  └─────────────────────────────────────────────────────────────┘        │
 │                                                                         │
 │  EndpointSlice (auto-managed by endpoint controller)                    │
 │  ┌─────────────────────────────────────────────────────────────┐        │
 │  │ endpoints:                                                  │        │
 │  │   - ip: 10.244.1.10   ready: true   nodeName: node-1       │        │
 │  │   - ip: 10.244.1.11   ready: true   nodeName: node-2       │        │
 │  │   - ip: 10.244.1.12   ready: true   nodeName: node-3       │        │
 │  │   - ip: 10.244.1.13   ready: false  nodeName: node-1  ─────┼──┐     │
 │  └─────────────────────────────────────────────────────────────┘  │     │
 └──────────────────────────────┬──────────────────────────────────  │ ────┘
                                │ watch stream                        │
                                │                                     │ not ready pods
                                ▼                                     │ are EXCLUDED
 ╔══════════════════════════════════════════════════════╗             │ from iptables
 ║         KUBE-PROXY  (DaemonSet, one per node)        ║  ◄─────────┘ rules
 ║                                                      ║
 ║  Watches API server for Service and                  ║
 ║  EndpointSlice changes via watch stream.             ║
 ║  On every change it rewrites iptables rules.         ║
 ║                                                      ║
 ║  iptables DNAT rules generated:                      ║
 ║                                                      ║
 ║  dst 10.0.1.42:8080                                  ║
 ║    → 33% chance → 10.244.1.10:8080  (pod 1)         ║
 ║    → 33% chance → 10.244.1.11:8080  (pod 2)         ║
 ║    → 33% chance → 10.244.1.12:8080  (pod 3)         ║
 ║                                                      ║
 ║  Algorithm: RANDOM with equal probability            ║
 ║  (pure random, not resource-aware)                   ║
 ╚══════════════════════════════════════════════════════╝


 Session Affinity on service definitions permit to lube proxy to decide which pod receives traffic:




HPA also helps doing this by simply scaling the pods when they are struggling.

Istio traffic policy in destination rules. Attached file istio-traffic-policy.yaml