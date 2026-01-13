Please setup the following on your local machine:

1 - Start by install az command line tool (f.e: using brew install az on MacOs)
2 - Login to your azure subscription
3 - Install kubectl command executable (f.e: using brew install kubectl on MacOs)
4 - Install helm (f.e: using brew install helm on MacOs)
5 - Get execute write on the cluster init script under the root of this folder (chmod +x aks_cluster_init.sh for Mac users)
6 - Now that you have the necessary rights: run the init shell script

Additional tools:
- Kubeseal command client (Used for creating sealed secrets on client side)

The init shell script will install all necessary prerequisites for the AKS cluster to bootstrap correctly. Dependencies include:

- Argocd for GitOps
- Prometheus CRDs which are necessary for startup