variable "environment" {
  type = string
  description = "Used mostly for resources naming"
}

variable "acr_registry_id" {
  type = string
  description = "Needed in order to setup security rules associated to the platform's Azure Container Registry"
}