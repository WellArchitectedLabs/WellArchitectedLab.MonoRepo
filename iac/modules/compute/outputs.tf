output "acr_resource_id" {
  value = azurerm_container_registry.acr-kubernetes-001.id
  description = "Output the ACR Id for usage in security and other purposes"
}

output "aks_resource_kubelet_object_id" {
  value = azurerm_kubernetes_cluster.aks-kubernetes-001.kubelet_identity[0].object_id
  description = "Output the AKS Kubelet identity object id in order to grant pods access to needed Azure resources"
}