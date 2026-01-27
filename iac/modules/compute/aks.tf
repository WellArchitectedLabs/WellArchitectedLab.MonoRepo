# resource "azurerm_virtual_network" "vnet-kubernetes-dev-001" {
#   address_space       = local.vnet_addressing_space
#   name                = "vnet-kubernetes-${var.environment}-001"
#   location            = var.region
#   resource_group_name = var.resource_group_name
# }

# resource "azurerm_subnet" "snet-kubernetes-dev-001" {
#   resource_group_name  = var.resource_group_name
#   name                 = "snet-kubernetes-${var.environment}-001"
#   address_prefixes     = ["10.0.1.0/24"]
#   virtual_network_name = azurerm_virtual_network.vnet-kubernetes-dev-001.name
#   # This is a dedicated private subnet for aks
#   # it permits to link aks with private azure services
#   # in our case, we would to link aks to the container registry for improved security
#   delegation {
#     name = "aksDelegation"
#     service_delegation {
#       name    = "Microsoft.ContainerService/managedClusters"
#       actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
#     }
#   }
# }

resource "azurerm_kubernetes_cluster" "aks-kubernetes-001" {
  name                = "aks-${var.environment}-001"
  location            = var.region
  resource_group_name = var.resource_group_name
  default_node_pool {
    name       = local.system_node_pool_name
    vm_size    = local.node_pools_vm_size
    node_count = 1
    # link aks cluster to the subnet in question
    # temporary_name_for_rotation = "npkubtemp"
    # vnet_subnet_id = azurerm_subnet.snet-kubernetes-dev-001.id
  }
  dns_prefix = "kubernetes"
  identity {
    type = "SystemAssigned"
  }
  depends_on = [azurerm_container_registry.acr-kubernetes-001]
}

resource "azurerm_kubernetes_cluster_node_pool" "npuser001" {
  kubernetes_cluster_id = azurerm_kubernetes_cluster.aks-kubernetes-001.id
  name                  = local.user_node_pool_name
  node_taints           = local.user_node_pool_taints
  node_count            = 1
  depends_on            = [azurerm_kubernetes_cluster.aks-kubernetes-001]
  vm_size               = local.node_pools_vm_size
  node_labels = {
    istio = local.istio_label_marker
  }
}

# It was decided, for workloads optimizations in regards to subscription quotas to use a single node pool for all user workloads
# Weither it is operations (istio, grafana, argocd ect..) or user applications (weather forecast bff or frontend ect..)
# They are live in the same node pool, in the same cluster
# So the frontend and operations nodepools were merged into a single node pool

# resource "azurerm_kubernetes_cluster_node_pool" "npuser002" {
#   kubernetes_cluster_id = azurerm_kubernetes_cluster.aks-kubernetes-001.id
#   name                  = "npuser002"
#   node_taints           = ["workload=frontend:NoSchedule"]
#   node_count            = 1
#   depends_on            = [azurerm_kubernetes_cluster.aks-kubernetes-001, azurerm_kubernetes_cluster_node_pool.npuser001]
#   vm_size               = "Standard_B2s_v2"
# }

# resource "azurerm_kubernetes_cluster_node_pool" "npoperations001" {
#   kubernetes_cluster_id = azurerm_kubernetes_cluster.aks-kubernetes-001.id
#   name = "npops001"
#   node_taints = ["workload=operations:NoSchedule"]
#   node_count = 1
#   depends_on = [ azurerm_kubernetes_cluster.aks-kubernetes-001, azurerm_kubernetes_cluster_node_pool.npuser002 ]
#   vm_size = "Standard_B2s_v2"
#   node_labels = {
#     workload = "operations"
#     istio    = "pilot"
#   }
# }

# # In our case: the consumer is the AKS cluster and the resource if the container resgitry
# # When this private endpoint config is applied, we can say that we enabled the AKS cluster integration with other azure services using private endpoints, which a majot security enhancement since we no longer need to exposes the dependant services publically to the internet
# # Please note the restriction on azure level that requires that the subnet running the consumer resource (the AKS cluster or App Service or any compute service) need to be 'delegated' for the given service. This means that the subnet will only support aks cluster resources and nothing else
# resource "azurerm_private_endpoint" "pe-aks-dev-001" {
#   resource_group_name = var.resource_group_name
#   location            = var.region
#   name                = "pe-aks-${var.environment}-001"
#   # We bind the aks cluster's subnet to the private endpoint using this line of code
#   # Please note that assigning a vnet direcly in function apps is not possible. this is why we may need azurerm_app_service_virtual_network_swift_connection for function apps integration with the subnet in question
#   # the same azurerm_app_service_virtual_network_swift_connection configuration for function app is replaced by the simple subnet assignment below
#   subnet_id = azurerm_subnet.snet-kubernetes-dev-001.id
#   # We bind the private endpoint to the ACR resource using this bloc
#   private_service_connection {
#     name                 = "sconnaks${var.environment}001"
#     is_manual_connection = false
#     # we specify which resource is bound to the this private endpoint
#     private_connection_resource_id = azurerm_container_registry.acr-kubernetes-001.id
#     # we also specify which are the potential resource types that can be added to this private endpoint too
#     subresource_names = ["registry"]
#   }
# }

# # After we bound aks to a subnet and make this subnet delegated for aks workloads
# # And after we bound that subnet to the ACR resource using private endpoints
# # The aks cluster can reach ACR privately via that private subnet, but AKS nodes will not have the luxery of quering the ACR FQDN anymore since the ACR cluster is now private
# # It is important then to add a new dns zone to the virtual network that resolve yto the services of type azure container registry
# # The dns zone name is standard and need to be named privatelink.azurecr.io for ACR registries (Azure has dedicated names for every service like privatelink.documents.azure.com for cosmos db ect...)
# resource "azurerm_private_dns_zone" "pzone-acr" {
#   # need to be exacly "privatelink.azurecr.io" for identifying the correct dns zone that resolves to private azure services
#   name                = "privatelink.azurecr.io"
#   resource_group_name = var.resource_group_name
# }

# resource "azurerm_private_dns_zone_virtual_network_link" "link-zone-acr-kubernetes-001" {
#   name                  = "link-acr-aks-${var.environment}"
#   private_dns_zone_name = azurerm_private_dns_zone.pzone-acr.name
#   # bind the dns zone to the virtual network, hosting the integration vnet
#   virtual_network_id  = azurerm_virtual_network.vnet-kubernetes-dev-001.id
#   resource_group_name = var.resource_group_name
# }