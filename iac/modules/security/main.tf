terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.1.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.50"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription
}

####################################################################################
# Enterprise App Creation
# The enterprise application will be used for CI/CD acr push rights assignment
####################################################################################

resource "azuread_application" "acr_push_app" {
  display_name = "${local.enterprise_application_name}-${var.environment}"
}

resource "azuread_application_password" "acr_push_app_password" {
  # secret is consultable in secrets screen on the enterprise application level
  application_id = azuread_application.acr_push_app.id
}

##################################################
# Associate a service principal to the enterprise app
##################################################

resource "azuread_service_principal" "acr_push_sp" {
  client_id = azuread_application.acr_push_app.client_id
}

##################################################
# Assign ACR Push role for the owner of the service principal
# (Final step in order for CI/CD pipeline to push artifacts into ACR registry)
##################################################

resource "azurerm_role_assignment" "acr_push" {
  principal_id         = azuread_service_principal.acr_push_sp.id
  role_definition_name = local.acr_push_role
  scope                = var.acr_registry_id
  depends_on           = [azuread_service_principal.acr_push_sp]
}

# Used for getting the tenant ID for outputs
data "azurerm_client_config" "current" {}