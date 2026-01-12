output "acr_push_user_name" {
  value       = module.compute.acr_push_user_name
  description = "Enterprise Application ID (used by github actions pipelines)"
}

output "acr_push_password" {
  value       = module.compute.acr_push_password
  description = "Enterprise Application Secret (used by github actions pipelines)"
  sensitive   = true
}

output "tenant_id" {
  value       = module.compute.tenant_id
  description = "Tenant ID to use for ACR authentication"
}