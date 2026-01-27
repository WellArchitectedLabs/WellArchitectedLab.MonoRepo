####################################################################################
# Assign AKS Kubelet identity access to the imports storage account
####################################################################################
resource "azurerm_role_assignment" "aks_storage_rw" {
  scope                = var.imports_storage_account_id
  role_definition_name = local.blob_data_contributor_role
  principal_id         = var.aks_resource_kubelet_identity_object_id
}