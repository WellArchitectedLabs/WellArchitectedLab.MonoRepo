#!/usr/bin/env bash
set -euo pipefail

FEED_NAME="GitHub"
ORG_NAME="WellArchitectedLabs"
FEED_URL="https://nuget.pkg.github.com/${ORG_NAME}/index.json"
SECRETS_DIR="${HOME}/.wellarchitectedlabs"
SECRETS_FILE="${SECRETS_DIR}/secrets.json"

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

########################################################################
## Create secrets file for Docker BuildKit
## Necessary for securely accessing GitHub Packages during Docker builds
########################################################################

mkdir -p "${SECRETS_DIR}"

cat > "${SECRETS_FILE}" <<EOF
{
  "github": {
    "username": "${GITHUB_USERNAME}",
    "token": "${GITHUB_TOKEN}"
  }
}
EOF

chmod 600 "${SECRETS_FILE}"

echo "Secrets file created at:"
echo "${SECRETS_FILE}"
echo ""
echo "You can now build using Docker BuildKit."

########################################################################
## Adding Github oackage source with PAT credentials 
## in local NuGet configuration file: ~/.nuget/NuGet/NuGet.Config
########################################################################

if dotnet nuget list source | grep -q "^  ${FEED_NAME}\s"; then
  dotnet nuget remove source "${FEED_NAME}"
fi

dotnet nuget add source "${FEED_URL}" \
  --name "${FEED_NAME}" \
  --username "${GITHUB_USERNAME}" \
  --password "${GITHUB_TOKEN}" \
  --store-password-in-clear-text

echo "GitHub NuGet source '${FEED_NAME}' configured successfully."
