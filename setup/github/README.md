# GitHub Packages – NuGet Setup Script

This repository provides a setup script that configures access to **GitHub Packages (NuGet)** for local development **without hardcoding PATs**, passwords, or secrets in source control.

It relies on **GitHub CLI (`gh`)** as the authentication authority and configures NuGet at the **user level**.

---

## What the script does

The script:

1. Verifies required tools (`gh`, `dotnet`)
2. Ensures the user is authenticated with GitHub CLI
3. Retrieves the active GitHub CLI token
4. Registers the GitHub Packages NuGet feed
5. Stores credentials **only in the user-level NuGet config**

No credentials are committed to the repository.

---

## Authentication model

- GitHub Packages **does not support interactive login** for NuGet
- Authentication is **token-based only**
- GitHub CLI already manages a secure token for the current user

The script simply bridges:

GitHub CLI authentication → NuGet authentication

### Zero-trust / least-privilege recommendation

Authenticate GitHub CLI with minimal scope:

```bash
gh auth login --scopes read:packages
```

This token is sufficient for dotnet restore.

## NuGet configuration resolution behavior

NuGet merges configuration files in a strict order.

Resolution precedence (lowest → highest):

- Machine-wide
- User-level
- Repository-level

Credentials are typically resolved from the user-level config.

### NuGet.config locations by OS
macOS

```bash
~/.nuget/NuGet/NuGet.Config
```

Linux

```bash
~/.nuget/NuGet/NuGet.Config
```

Windows

```bash
%AppData%\NuGet\NuGet.Config
```


### Expanded example:

What the script modifies
The script writes only to the user-level NuGet config file.
It adds (or replaces) a source similar to:

```xml
<packageSources>
  <add key="GitHub"
       value="https://nuget.pkg.github.com/WellArchitectedLabs/index.json" />
</packageSources>

<packageSourceCredentials>
  <GitHub>
    <add key="Username" value="<your-github-username>" />
    <add key="ClearTextPassword" value="<token-from-gh-cli>" />
  </GitHub>
</packageSourceCredentials>
```

*The token is sourced dynamically from GitHub CLI.*

## How the script works

When the script runs: gh auth token, it returns the token currently active for github.com, with the scopes granted during gh auth login, 

**the script does not:**
- Enumerate tokens
- Choose a token by name
- Access CI secrets

*Sure thing, CI tokens are not visible locally.*

## Verifying configuration

To list configured NuGet sources:

```bash
dotnet nuget list source --format detailed
```

This shows:

- Source name
- URL

## CI behavior (for reference)

This script **is not and should not be used in CI**.
This script is only a local development setup script.
GitHub Actions use GITHUB_TOKEN.