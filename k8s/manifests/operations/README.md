Sealed secrets command:

kubeseal \
  --controller-name sealed-secrets \
  --controller-namespace kube-system \
  --namespace monitoring \
  --scope namespace-wide \
  --format yaml \ 
  < secret.yaml > sealed-secret.yaml

If not specified kubeseal executable will create the sealed secret in strict mode (namespace + cluster name + controller version). We don't want controller version check since controller can evolve so we just use namespace wide mode