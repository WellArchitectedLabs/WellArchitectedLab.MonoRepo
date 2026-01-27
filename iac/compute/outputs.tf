output "acr_resource_id" {
  value = azurerm_container_registry.acr-kubernetes-001.id
  description = "Output the ACR Id for usage in security and other purposes"
}

output "acr_push_user_name" {
  value = azuread_application.acr_push_app.client_id
  description = "Enterprise Application ID (used by github actions pipelines)"
}

output "acr_push_password" {
  value = azuread_application_password.acr_push_app_password.value
  description = "Enterprise Application Secret (used by github actions pipelines)"
  sensitive = true
}

output "tenant_id" {
  value = data.azurerm_client_config.current.tenant_id
  description = "Tenant ID to use for ACR authentication"
}