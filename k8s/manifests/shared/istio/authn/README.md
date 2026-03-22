┌─────────────────────────────────────────────────────────────┐
│                  Istiod (control plane)                     │
│              Issues SVID certs via SPIFFE                   │
└───────────────────┬─────────────────────┬───────────────────┘
                    │ cert                │ cert
                    ▼                     ▼
┌───────────────────────────┐   ┌───────────────────────────┐
│       Service A pod       │   │       Service B pod       │
│  ┌──────────┐ ┌────────┐  │   │  ┌────────┐ ┌──────────┐ │
│  │   App    │ │ Envoy  │  │   │  │ Envoy  │ │   App    │ │
│  │         ─┼─► sidecar│  │   │  │sidecar─┼─►         │ │
│  │(plaintext│ │        │  │   │  │        │ │(plaintext│ │
│  └──────────┘ └───┬────┘  │   │  └────┬───┘ └──────────┘ │
└───────────────────┼───────┘   └───────┼───────────────────┘
                    │                   │
                    │   mTLS (TLS 1.3)  │
                    │  mutual SVID verify│
                    └───────────────────┘

Key resources:
  PeerAuthentication  STRICT       → what the receiver accepts
  DestinationRule     ISTIO_MUTUAL → how the sender connects
  Envoy sidecar                    → terminates TLS transparently

SPIFFE SVID (SPIFFE Verifiable Identity Document)
  A short-lived X.509 certificate issued by Istiod to every
  sidecar-injected pod. It encodes the workload identity as a
  URI in the SAN field:

    spiffe://cluster.local/ns/<namespace>/sa/<service-account>

  Both sides of a connection present their SVID during the TLS
  handshake. Each verifies the other against the shared trust
  bundle (the mesh CA). This mutual verification is what makes
  it mTLS rather than one-way TLS.

  Properties:
    Issuer     → Istiod acting as mesh CA
    Identity   → Kubernetes ServiceAccount (not pod name/IP)
    Format     → X.509 certificate in SAN URI field
    Lifetime   → 24h by default, auto-rotated by Istiod
    Scope      → one SVID per pod, shared by all containers in it
    Used by    → AuthorizationPolicy principal matching:
                 cluster.local/ns/prod/sa/orders-service