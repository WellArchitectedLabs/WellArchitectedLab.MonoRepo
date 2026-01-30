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
  subscription_id = local.subscription
}

module "global" {
  source       = "../../modules/global"
  region       = local.region
  subscription = local.subscription
  environment  = local.environment
}

module "compute" {
  source              = "../../modules/compute"
  region              = local.region
  subscription        = local.subscription
  resource_group_name = module.global.resource_group_name
  environment         = local.environment
}

module "storage" {
  source = "../../modules/storage"
  region = local.region
  environment = local.environment
  resource_group_name = module.global.resource_group_name
}

module "security" {
  source = "../../modules/security"
  environment = local.environment
  acr_resource_id = module.compute.acr_resource_id
  subscription = local.subscription
  aks_resource_kubelet_identity_object_id = module.compute.aks_resource_kubelet_object_id
  imports_storage_account_id = module.storage.imports_storage_account_resource_id
}
