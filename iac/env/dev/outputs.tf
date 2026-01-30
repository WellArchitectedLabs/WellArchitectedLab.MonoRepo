output "tenant_id" {
  value       = module.security.tenant_id
  description = "Tenant ID to use for ACR authentication"
}

output "acr_resource_id" {
  value       = module.compute.acr_resource_id
  description = "ACR Resource Id"
}

output "acr_push_user_name" {
  value       = module.security.acr_push_user_name
  description = "Enterprise Application ID (used by github actions pipelines)"
}

output "acr_push_password" {
  value       = module.security.acr_push_password
  description = "Enterprise Application Secret (used by github actions pipelines)"
  sensitive   = true
}