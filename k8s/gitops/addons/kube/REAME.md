Contains are kubernetes controller extensions.
For example, we manage kubeseal controller via a separate argocd application.
Usage of kubeseal on client side (needs client installation):
kubeseal --controller-name=sealed-secrets --controller-namespace=kube-system --format=yaml < secrets.yaml > sealed-secrets.yaml