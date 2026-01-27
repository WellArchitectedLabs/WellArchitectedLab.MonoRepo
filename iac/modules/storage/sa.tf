resource "azurerm_storage_account" "stwfimports" {
  name = "${local.imports_storage_account_prefix}${var.environment}"
  account_tier = local.standard_storage_account_tier
  resource_group_name = var.resource_group_name
  location = var.region
  account_replication_type = local.lrs_replication_tier
   tags = {
    environment = var.environment
  }
}