locals {
  acr_standard_tier_name = "Standard"
  node_pools_vm_size = "Standard_B2s_v2"
  system_node_pool_name = "npkube001"
  user_node_pool_name = "npuser001"
  user_node_pool_taints = ["workload=weather-forecast:NoSchedule"]
  istio_label_marker = "pilot"
}