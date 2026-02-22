DOCKER_BUILDKIT=1 docker build \
  --secret id=dev_secrets,src=$HOME/.wellarchitectedlabs/secrets.json \
  -t wfinsights-collector \
  -f Dockerfile.insights-collector