#!/usr/bin/env bash
set -euo pipefail

# Usage
if [ $# -ne 2 ]; then
  echo "Usage: $0 <resource_group> <cluster_name>"
  exit 1
fi

RESOURCE_GROUP="$1"
CLUSTER_NAME="$2"
ARGOCD_NAMESPACE="argocd"

echo "=== Bootstrap: Prometheus CRDs + Argo CD ==="

# Ensure Azure CLI
if ! command -v az >/dev/null; then
  echo "❌ Azure CLI not found. Install Azure CLI first."
  exit 1
fi

# Get AKS credentials
echo "→ Fetching AKS credentials..."
az aks get-credentials \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CLUSTER_NAME" \
  --overwrite-existing

# Verify kubectl
echo "→ Checking Kubernetes access..."
kubectl get ns >/dev/null

# Ensure kubectl & helm installed
if ! command -v kubectl >/dev/null; then
  echo "❌ kubectl not found. Please install kubectl."
  exit 1
fi

if ! command -v helm >/dev/null; then
  echo "❌ helm not found. Please install Helm 3."
  exit 1
fi

# -----------------------
# Install Prometheus CRDs
# -----------------------

echo "→ Installing Prometheus Operator CRDs (via helm template)..."

helm repo add prometheus-community https://prometheus-community.github.io/helm-charts >/dev/null 2>&1 || true
helm repo update >/dev/null

helm template prometheus-crds prometheus-community/prometheus-operator-crds \
  | kubectl apply --server-side -f -

echo "✅ Prometheus CRDs installed successfully"