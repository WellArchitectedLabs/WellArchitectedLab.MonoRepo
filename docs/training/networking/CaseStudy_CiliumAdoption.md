**Azure CNI** — Each pod gets a real VNet IP pre-allocated from the subnet. The upside is direct routability from anywhere in the VNet without encapsulation. The downside is exactly what you said: IP exhaustion is a real operational problem because you need to pre-reserve IPs per node, and large clusters eat subnets fast. There's a newer variant called Azure CNI Overlay that Microsoft added specifically to address this, which gives pods IPs from a private overlay range while keeping node IPs in the VNet.
**Kubenet** — Kubenet is not really a "network management system" — it's a very minimal CNI plugin that ships with Kubernetes itself. It does almost nothing on its own. What makes it work on AKS is that Azure automatically manages UDR (User Defined Routes) in the route table for each node's pod CIDR. So when a packet arrives at a node destined for a pod on another node, the Azure route table says "send it to node X" and then kubenet/Linux routes it to the right pod via a local bridge (cbr0). You're right that it uses an overlay-like approach with private pod CIDRs, enabling far more pods per node since you're not consuming real VNet IPs per pod.
The subtle distinction: kubenet itself is just the basic CNI plumbing. The "magic" that makes cross-node routing work is Azure's route table management, not kubenet itself.


Two levels of networking:

1 - Cluster level networking: manages IP address allocation and cross-node routing.
  - Kubenet: pods get IPs from a private overlay CIDR (not VNet IPs), enabling
    more pods per node without VNet IP exhaustion. Cross-node routing relies on
    Azure UDR route tables. Slightly less performant than Azure CNI due to the
    extra routing hop, but the gap is modest in practice.
  - Azure CNI (default): each pod gets a real VNet IP pre-allocated from the
    subnet. Fully routable within the VNet without encapsulation, but consumes
    VNet IPs rapidly — IP exhaustion is the main operational drawback.
  - Azure CNI Overlay (newer): combines the best of both — pods get IPs from a
    private overlay range (no VNet IP exhaustion) while keeping Azure CNI's
    superior routing performance. Replaces kubenet as the recommended overlay
    option on AKS.

2 - Pod level networking: covers two distinct concerns that are often confused:

  a) Service routing (ClusterIP → pod IP translation):
     Managed by kube-proxy via iptables/ipvs rules on each node.
     kube-proxy has no role in direct pod-to-pod communication.
     Cilium can fully replace kube-proxy for this using eBPF,
     independently of how it is deployed as a CNI.

  b) Pod-to-pod policy enforcement (L4/L7 restrictions):
     Managed by the CNI plugin, not kube-proxy.
     Cilium adds rich L4/L7 CiliumNetworkPolicy enforcement on top
     of whichever cluster networking mode is in use.

  The most common Cilium setup on AKS is CNI chaining with Azure CNI:
  Azure CNI remains responsible for IP allocation and east-west routing,
  and Cilium attaches eBPF programs on top solely for policy enforcement.
  kube-proxy replacement is an independent choice in this setup —
  you can chain with Azure CNI and still replace kube-proxy with Cilium,
  or keep kube-proxy. The two settings do not affect each other.

  When chainingMode is none, Cilium acts as the sole CNI and takes full
  ownership of IP assignment, east-west routing, and policy enforcement,
  replacing Azure CNI. kube-proxy may still be replaced independently.

  The most common mode for adopting cillium as for network plicies enforcements is adopting on custer creation.
  This is the most stable, azure compatible mode.
  Custom Cillium installations like the one coded in the application.yaml file under this same folder is very advanced, yet not needed for our setup.
  As part of this project, and since the cluster was created using default networking component (kube-proxy), and for sake of simplicity, the decision was made in order to stay with built in network policies
  They stay safe, production ready, a bit more hard to maintain and to write, for sure, but still production ready and very relevant for our use case.
