resource "azurerm_resource_group" "rg-kubernetes-001" {
  name     = "rg-kubernetes-${var.environment}-001"
  location = var.region
}