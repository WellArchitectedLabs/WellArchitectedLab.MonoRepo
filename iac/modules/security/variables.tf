#####################
### Global level vars
#####################

variable "environment" {
  type = string
  description = "Used mostly for resources naming"
}

variable "subscription" {
  type        = string
  description = "Azure subscription in which resources will be billed"
}

#####################################################
### Resources ids. Necessary to implement RBAC access
#####################################################

variable "acr_resource_id" {
  type = string
  description = "Needed in order to setup security rules associated to the platform's Azure Container Registry"
}

variable "aks_resource_kubelet_identity_object_id" {
  type = string
  description = "Needed in order to setup security rules associated to the platform's Azure Kubernetes Service"
}

variable "imports_storage_account_id" {
  type = string
  description = "Needed in order to setup security rules associated to the imports workloads"
}