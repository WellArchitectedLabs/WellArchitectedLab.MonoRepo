# Master Data Client (.NET)

## Overview
**InsightsCollector.Client** is the official .NET client library providing a clean, typed connector to the **Insights Collector API**.  
It follows a **Bring Your Own Client (BYOC)** model — integrating seamlessly into your application runtime while allowing you to maintain full control over HTTP configuration, hosting, and lifecycle.

> **Note:** This package is designed exclusively for .NET client applications.  
> Connectors for other languages may be introduced in the future.

---

## Prerequisites
- **.NET 10 or higher**  
- **Network access** to the Insights Collector API over a private network  
- **HTTP/1.1 support**  
- **Ability to issue GET requests**

---

## Supported Scenarios
- .NET client applications (ASP.NET Core, worker services, background jobs)  
- Dependency Injection–based application architectures  
- Private network deployments  
- Controlled enterprise environments  

> This package is **not intended** for server-side SDK aggregation or non-.NET runtimes.

---

## Installation

### 1. Add the GitHub Packages NuGet source

```bash
dotnet nuget add source \
  https://nuget.pkg.github.com/WellArchitectedLab/index.json \
  --name github
```

### 2. Reference the package 
- Install the latest version of the client library:

```bash
dotnet add package InsightsCollector.Client
```

## Usage

### Registering the Client

- The client integrates via Dependency Injection.
- Use the provided extension method to register the Master Data client and configure options as needed:

```csharp
services.RegisterInsightsCollectorClient(options =>
{
    // Configure endpoint, authentication, or HTTP settings if required
});
```
*Once registered, the client can be injected and used throughout your application.*

## Runtime Behavior

- Uses HTTP/1.1
- REST style API
- Designed for private network communication
- Relies on the hosting runtime (the dotnet kernel) for transport and lifecycle management

## Versioning Strategy

This package follows Semantic Versioning (SemVer):

```text
MAJOR.MINOR.PATCH
```

What This Means for Consumers:

### PATCH releases

- Contain bug fixes and performance improvements
- Safe to upgrade automatically

### MINOR releases

- Add backward-compatible features
- Safe to upgrade automatically

### MAJOR releases

- Introduces breaking changes
- Require explicit consumer action

## Breaking Changes Communication

### In the event of a breaking change:

- A major version bump will be published
- Release notes will describe the impact clearly
- Email communication will be sent to consumers three mounths before the change is applied and the old versions are deprecated.

### Consumers are encouraged to:

- Automatically accept patch and minor updates
- Review release notes before upgrading to a major version

### Upgrading

- Always refer to release notes before upgrading
- Breaking changes occur only in major versions 
- Minor and patch updates are safe by design

### Support and Roadmap

- This package targets .NET clients only
- Additional language connectors may be developed in the future
- Issues and feature requests should be raised via the repositoy

### License

This project is licensed under the terms specified in the package metadata.