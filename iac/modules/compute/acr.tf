resource "random_string" "acr_suffix" {
  length  = 6
  special = false
  lower   = true
  numeric = true
}

resource "azurerm_container_registry" "acr-kubernetes-001" {
  name                = "acr${var.environment}${random_string.acr_suffix.result}"
  sku                 = local.acr_standard_tier_name
  resource_group_name = var.resource_group_name
  location            = var.region
  admin_enabled       = false
}