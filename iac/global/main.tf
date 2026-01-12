terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.1.0"
    }
  }
  backend "azurerm" {
    key                  = "kubernetes.terraform.tfstate"
    storage_account_name = "devwaltstate"
    resource_group_name  = "dev-rg-terraforminit"
    container_name       = "tfstate"
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription
}

resource "azurerm_resource_group" "rg-kubernetes-001" {
  name     = "rg-kubernetes-${var.environment}-001"
  location = var.region
}