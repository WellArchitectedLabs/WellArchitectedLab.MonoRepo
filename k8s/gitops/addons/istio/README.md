Istio components execution order (synced by argocd waves):

1 - Istio base (istio crds)
2 - Istiod (the istio control plane that lives into the node pools and that injects and controls the sidecars)
3 - Istio Ingress Gateway (The ingress gateway components that intercepts requests by exposing hosts and ports)
4 - Pod monitor: necessary to activate the prometheus istio metrics scrapping agent