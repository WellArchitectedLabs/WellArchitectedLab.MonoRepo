locals {
  acr_push_role = "AcrPush"
  enterprise_application_name= "ea-acr-push-weather-forecast"
  # ensure data read / write role on a given storage account (data only)
  blob_data_contributor_role= "Storage Blob Data Contributor"
  blob_account_key_operator_service_role = "Storage Account Key Operator Service Role"
  acr_pull_role = "AcrPull"
}