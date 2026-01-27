Very important to make a point here about:

- What is a CSI driver (runtime for how Kubernetes talks to Azure)
- What is a persistent volume (and why we do not declare it here for static blob allocation)
- Difference between static and dynamic blob integration
- What is a storage class (How does kubernetes create the volume. Plugs directly with the csi driver.)
- What is a persistent volume claim (A request + binding to that storage)

Important details:

- Azure blob storage data is not stored in the persistent volume. The persistent volume claims only binds the workloads to the external storage.
- What a PersistentVolume really is (with CSI)

A cluster object that describes how to reach some storage, not where data lives.
For CSI-backed storage:
Data lives outside Kubernetes (Azure Blob, Disk, File, etc.)

The PV stores:
CSI driver name
Volume handle (ID)
Parameters (protocol, account, container, etc.)
Reclaim policy
Access modes

It is essentially:

A pointer + contract

- Mantra: A PersistentVolume is a Kubernetes metadata object that represents an externally provisioned storage endpoint, usually created automatically by a CSI driver; a PVC is the abstraction Pods use to bind to that endpoint via a StorageClass.

One-line definitions
- CSI driver → Runtime implementation. Executes the real volume mounting to pods in low level details (protocols, auth ect..)
- StorageClass → Provisioning recipe (Defines how we connect to a certain storage that is needed for some workloads)
- PersistentVolume → Concrete storage reference (This is a reference. It does not store the real data. The metadata include the endpoint and some details that are used by the CSI driver for doing the mounting)
- PersistentVolumeClaim → Binding contract (The consumer needs a contract for accessing that persistent volume. Contract should be satisfied by the cluster)
- Pod → Consumer