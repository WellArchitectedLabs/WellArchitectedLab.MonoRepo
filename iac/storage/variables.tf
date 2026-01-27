variable "environment" {
  type = string
  description = "Used mostly for resources naming"
}

variable "resource_group_name" {
  type = string
  description = "Resource group in which we will create storage resources"
}

variable "region" {
  type = string
  description = "Region in which we will create the storage resources"
}