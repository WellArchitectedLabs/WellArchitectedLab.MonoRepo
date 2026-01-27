output "acr_resource_id" {
  value = azurerm_container_registry.acr-kubernetes-001.id
  description = "Output the ACR Id for usage in security and other purposes"
}