output "app_service_url" {
  value = azurerm_app_service.useless-resource.default_site_hostname
}
