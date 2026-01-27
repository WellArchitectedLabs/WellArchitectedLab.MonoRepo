terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.1.0"
    }
  }
}

resource "azurerm_storage_account" "stwfimports" {
  name = "stwfimports${var.environment}"
  account_tier = "Standard"
  resource_group_name = var.resource_group_name
  location = var.region
  account_replication_type = "LRS"
   tags = {
    environment = var.environment
  }
}