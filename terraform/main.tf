terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  
  backend "azurerm" {
    resource_group_name  = "rg-useless-api"
    storage_account_name = "tfstateuselessapi"
    container_name      = "tfstate"
    key                 = "terraform.tfstate"
  }
}

variable "image_tag" {}
variable "acr_name" {}
variable "image_name" {}

resource "azurerm_service_plan" "app_plan" {
  name                = "useless-plan-linux"
  location            = "eastus"
  resource_group_name = "rg-useless-api"
  os_type            = "Linux"
  sku_name           = "B1"
}

resource "azurerm_linux_web_app" "app" {
  name                = "useless-api-linux"
  location            = "eastus"
  resource_group_name = "rg-useless-api"
  service_plan_id     = azurerm_service_plan.app_plan.id

  site_config {
    application_stack {
      docker_image     = "${var.acr_name}/${var.image_name}"
      docker_image_tag = var.image_tag
    }
  }

  app_settings = {
    "DOCKER_REGISTRY_SERVER_URL"          = "https://${var.acr_name}"
    "DOCKER_REGISTRY_SERVER_USERNAME"     = var.acr_name
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/acr-password)"
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
  }
}