The components involved
CoreDNS is a pod (actually a Deployment) running in kube-system. It is the cluster's DNS server. Every DNS query from any pod in the cluster is sent to CoreDNS.
/etc/resolv.conf is injected into every pod automatically by the kubelet when the pod starts. It tells the pod where to send DNS queries. It looks like this inside any pod:
nameserver 10.0.0.10       # CoreDNS ClusterIP
search default.svc.cluster.local svc.cluster.local cluster.local
options ndots:5
CoreDNS ClusterIP is the stable virtual IP assigned to the kube-dns Service in kube-system. It never changes for the lifetime of the cluster. kube-proxy maintains the iptables rule that forwards traffic to this IP to whichever CoreDNS pod is currently running.

What happens when a pod calls another service by name
Let's trace exactly what happens when a pod in forecast-experience calls http://master-data-svc:8080:
Step 1 — The application makes a DNS query
The pod's network library reads /etc/resolv.conf and sees nameserver 10.0.0.10. It sends a UDP packet to 10.0.0.10:53 asking for the A record of master-data-svc.
Step 2 — ndots:5 search path expansion
Before sending, the resolver checks the ndots:5 option. This means: if the hostname has fewer than 5 dots, try appending the search domains first before treating it as an absolute name. So master-data-svc gets expanded and queried in this order:
master-data-svc.default.svc.cluster.local      # tries namespace first
master-data-svc.svc.cluster.local
master-data-svc.cluster.local
master-data-svc.                               # bare name, tried last
This is why you can call services by short name within the same namespace — the search path resolves it. Calling across namespaces requires either the full name master-data-svc.master-data.svc.cluster.local or at least master-data-svc.master-data.
Step 3 — kube-proxy intercepts the UDP packet
The UDP packet is destined for 10.0.0.10:53 which is a ClusterIP — a virtual IP that no real network interface owns. kube-proxy has set up an iptables DNAT rule that intercepts this packet and rewrites the destination to the real IP of one of the CoreDNS pods, for example 10.244.0.5:53.
Step 4 — CoreDNS receives the query
CoreDNS receives the query for master-data-svc.master-data.svc.cluster.local. It looks this up in its internal cache of Kubernetes Service records, which it keeps in sync by watching the Kubernetes API server via a watch connection. It finds the ClusterIP for master-data-svc — say 10.0.1.42 — and returns it in the DNS response.
Step 5 — The pod receives the IP and opens a TCP connection
The pod now knows master-data-svc = 10.0.1.42. It opens a TCP connection to 10.0.1.42:8080. kube-proxy again intercepts this via iptables, DNATs it to a real pod IP behind the service, and the request reaches master-data.

Why this breaks without the DNS egress policy
When you apply a default-deny egress policy, step 1 is blocked at the eBPF or iptables level before the packet even leaves the pod's network namespace. The pod sends a UDP packet to 10.0.0.10:53 and it is silently dropped.
The application then waits for a DNS response that never comes, times out (usually after 5 seconds per search domain, so up to 30 seconds total across all search domains), and throws a Name or service not known error. From the application's perspective it looks like the downstream service is down, but the actual problem is that it never even got an IP to connect to.


╔══════════════════════════════════════════════════════════════════════════════════════════════╗
║                              KUBERNETES DNS RESOLUTION FLOW                                  ║
╚══════════════════════════════════════════════════════════════════════════════════════════════╝


 ┌─────────────────────────────────────────────────────┐
 │         POD  (forecast-experience namespace)         │
 │                                                     │
 │   app calls: http://master-data-svc:8080            │
 │                                                     │
 │   /etc/resolv.conf (injected by kubelet at start)   │
 │   ┌─────────────────────────────────────────────┐   │
 │   │ nameserver 10.0.0.10                        │   │
 │   │ search default.svc.cluster.local            │   │
 │   │        svc.cluster.local                    │   │
 │   │        cluster.local                        │   │
 │   │ options ndots:5                             │   │
 │   └─────────────────────────────────────────────┘   │
 └───────────────────────┬─────────────────────────────┘
                         │
                         │  STEP 1 — ndots:5 expands the name
                         │  tries in order:
                         │  1. master-data-svc.default.svc.cluster.local
                         │  2. master-data-svc.svc.cluster.local
                         │  3. master-data-svc.cluster.local
                         │
                         │  UDP packet → dst: 10.0.0.10:53
                         │
                         ▼
 ╔═══════════════════════════════════════════════════╗
 ║           NODE IPTABLES  (kube-proxy)             ║
 ║                                                   ║
 ║   DNAT rule:                                      ║
 ║   dst 10.0.0.10:53 → rewrite to CoreDNS pod IP   ║
 ║   e.g. 10.244.0.5:53                             ║
 ║                                                   ║
 ║   ⚠ THIS IS WHERE DNS EGRESS POLICY IS ENFORCED  ║
 ║   If egress UDP:53 is denied → packet dropped     ║
 ║   here and pod never gets a response              ║
 ╚═══════════════════════════════════════════════════╝
                         │
                         │  UDP packet → dst: 10.244.0.5:53  (real CoreDNS pod)
                         │
                         ▼
 ┌─────────────────────────────────────────────────────┐
 │              CoreDNS pod (kube-system)              │
 │                                                     │
 │   Receives query:                                   │
 │   master-data-svc.master-data.svc.cluster.local     │
 │                                                     │
 │   Looks up its internal cache ──────────────────────┼──────────────────────────┐
 │   (synced with API server via watch)                │                          │
 │                                                     │                          ▼
 │   Found: ClusterIP = 10.0.1.42                      │         ┌────────────────────────────┐
 │                                                     │         │   KUBERNETES API SERVER    │
 │   Returns DNS response:                             │         │                            │
 │   master-data-svc → 10.0.1.42                       │         │  CoreDNS watches Services  │
 │                                                     │         │  and Endpoints via a       │
 │   TCP fallback (port 53 TCP):                       │         │  persistent watch stream   │
 │   used only if response > 512 bytes                 │         │  to keep its cache in sync │
 │   (DNSSEC, large record sets)                       │         └────────────────────────────┘
 └───────────────────────┬─────────────────────────────┘
                         │
                         │  DNS response: 10.0.1.42
                         │
                         ▼
 ┌─────────────────────────────────────────────────────┐
 │         POD  (forecast-experience namespace)         │
 │                                                     │
 │   Resolved: master-data-svc = 10.0.1.42             │
 │   Opens TCP connection → dst: 10.0.1.42:8080        │
 └───────────────────────┬─────────────────────────────┘
                         │
                         │  TCP SYN → dst: 10.0.1.42:8080  (ClusterIP, virtual)
                         │
                         ▼
 ╔═══════════════════════════════════════════════════╗
 ║           NODE IPTABLES  (kube-proxy)             ║
 ║                                                   ║
 ║   DNAT rule:                                      ║
 ║   dst 10.0.1.42:8080 → rewrite to real pod IP    ║
 ║   e.g. 10.244.1.12:8080  (load balanced across   ║
 ║   all healthy pods behind the Service)            ║
 ╚═══════════════════════════════════════════════════╝
                         │
                         │  TCP → dst: 10.244.1.12:8080  (real pod IP)
                         │
                         ▼
 ┌─────────────────────────────────────────────────────┐
 │           POD  (master-data namespace)              │
 │                                                     │
 │   Receives request on port 8080                     │
 │   Processes and responds                            │
 └─────────────────────────────────────────────────────┘


══════════════════════════════════════════════════════════════════════════════════════════════
 WHAT BREAKS WITHOUT THE DNS EGRESS POLICY
══════════════════════════════════════════════════════════════════════════════════════════════

 pod sends UDP:53 ──► DROPPED by egress policy ──► no response
                                                          │
                       retries all search domains ◄───────┘
                       (up to 30s total timeout)
                                │
                                ▼
                    ✗ Name or service not known
                      (looks like downstream is down,
                       actual cause is DNS never resolved)