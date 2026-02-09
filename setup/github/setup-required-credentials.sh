#!/usr/bin/env bash
set -euo pipefail

FEED_NAME="GitHub"
ORG_NAME="WellArchitectedLabs"
FEED_URL="https://nuget.pkg.github.com/${ORG_NAME}/index.json"

command -v gh >/dev/null 2>&1 || {
  echo "GitHub CLI (gh) is required. Install it first."
  exit 1
}

command -v dotnet >/dev/null 2>&1 || {
  echo ".NET SDK is required. Install it first."
  exit 1
}

if ! gh auth status >/dev/null 2>&1; then
  echo "You are not logged in to GitHub CLI."
  gh auth login --scopes "read:packages"
fi

GITHUB_USERNAME="$(gh api user --jq .login)"
GITHUB_TOKEN="$(gh auth token)"

if dotnet nuget list source | grep -q "^  ${FEED_NAME}\s"; then
  dotnet nuget remove source "${FEED_NAME}"
fi

dotnet nuget add source "${FEED_URL}" \
  --name "${FEED_NAME}" \
  --username "${GITHUB_USERNAME}" \
  --password "${GITHUB_TOKEN}" \
  --store-password-in-clear-text

echo "GitHub NuGet source '${FEED_NAME}' configured successfully."
