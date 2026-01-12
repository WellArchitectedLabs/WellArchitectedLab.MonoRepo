The developed terraform based on a shared terraform state that lives in a dedicated container.
We assume you have done the following preparations:
- Make sure you can login in your azure subscription using az login
- Prepare a resource group named "dev-rg-terraforminit". 
- Prepare a storage account named "devwaltstate".
- In which you create a container named "tfstate". Terraform state will live in this container.
- Make sure the resources are created with the exact names as specified in this pre-requisites file.
- Install terraform.
- Run terraform validate / plan / apply.

-----------------------

After applying terraform changes using terraform apply, the script will output the following:
- A client id: this is the client for the application registration that holds the ACR Push role for the created cluster
- A password: this is the secret used by github for pushing in Azure Container registry
- And a tenant Id (Optional): is not directly used by github task but can be useful for debugging