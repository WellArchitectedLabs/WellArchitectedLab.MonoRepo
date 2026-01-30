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

##############################################################################
# Assign ACR Push role for the owner of the service principal
# (Final step in order for CI/CD pipeline to push artifacts into ACR registry)
##############################################################################

resource "azurerm_role_assignment" "acr_push" {
  principal_id         = azuread_service_principal.acr_push_sp.id
  role_definition_name = local.acr_push_role
  scope                = var.acr_resource_id
  depends_on           = [azuread_service_principal.acr_push_sp]
}

######################################################################
# Assign ACR Pull role for the kubelet identity (used by the AKS pods)
# To the ACR resource
######################################################################

resource "azurerm_role_assignment" "cluster-registry-access" {
  principal_id                     = var.aks_resource_kubelet_identity_object_id
  scope                            = var.acr_resource_id
  role_definition_name             = local.acr_pull_role
  skip_service_principal_aad_check = true
}

# Used for getting the tenant ID in outputs file
data "azurerm_client_config" "current" {}