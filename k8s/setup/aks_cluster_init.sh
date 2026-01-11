#!/usr/bin/env bash
set -euo pipefail

# Usage
if [ $# -ne 3 ]; then
  echo "Usage: $0 <resource_group> <cluster_name> <acr_name_or_id>"
  exit 1
fi

RESOURCE_GROUP="$1"
CLUSTER_NAME="$2"
ACR="$3"
ARGOCD_NAMESPACE="argocd"
ARGOCD_RELEASE_NAME="argocd"

echo "=== Bootstrap: AKS + ACR + Prometheus CRDs + Argo CD ==="

# Ensure Azure CLI
if ! command -v az >/dev/null; then
  echo "❌ Azure CLI not found. Install Azure CLI first."
  exit 1
fi

# Ensure kubectl
if ! command -v kubectl >/dev/null; then
  echo "❌ kubectl not found. Please install kubectl."
  exit 1
fi

# Ensure helm
if ! command -v helm >/dev/null; then
  echo "❌ helm not found. Please install Helm 3."
  exit 1
fi

# -----------------------
# Attach ACR to AKS
# -----------------------

echo "→ Attaching ACR to AKS (if not already attached)..."

az aks update \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CLUSTER_NAME" \
  --attach-acr "$ACR" \
  >/dev/null

echo "✅ ACR attached to AKS"

# -----------------------
# Get AKS credentials
# -----------------------

echo "→ Fetching AKS credentials..."

az aks get-credentials \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CLUSTER_NAME" \
  --overwrite-existing

# Verify kubectl access
echo "→ Checking Kubernetes access..."
kubectl get ns >/dev/null

# -----------------------
# Install Prometheus CRDs
# -----------------------

echo "→ Installing Prometheus Operator CRDs..."

helm repo add prometheus-community https://prometheus-community.github.io/helm-charts >/dev/null 2>&1 || true
helm repo update >/dev/null

helm template prometheus-crds prometheus-community/prometheus-operator-crds \
  | kubectl apply --server-side -f -

echo "✅ Prometheus CRDs installed successfully"

# -----------------------
# Install Argo CD
# -----------------------

echo "→ Installing Argo CD via Helm..."

helm repo add argo https://argoproj.github.io/argo-helm >/dev/null 2>&1 || true
helm repo update >/dev/null

kubectl get ns "$ARGOCD_NAMESPACE" >/dev/null 2>&1 || \
  kubectl create ns "$ARGOCD_NAMESPACE"

helm upgrade --install "$ARGOCD_RELEASE_NAME" argo/argo-cd \
  --namespace "$ARGOCD_NAMESPACE" \
  --wait

echo "✅ Argo CD installed successfully"
echo "=== Bootstrap completed ==="
