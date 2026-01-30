Sync waves are used in order to order application deployments.
Here are the sync code ranges:

Istio: from 0 to 10
Grafana: from 10 to 20
Kube: from -20 to -1

kubectl -n argocd get secret argocd-initial-admin-secret \
          -o jsonpath="{.data.password}" | base64 -d; echo