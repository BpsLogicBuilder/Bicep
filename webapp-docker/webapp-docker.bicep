@description('Generate a Suffix based on the Resource Group ID')
param suffix string = uniqueString(resourceGroup().id)

@description('Use the Resource Group Location')
param location string = resourceGroup().location

resource acr 'Microsoft.ContainerRegistry/registries@2025-11-01' existing = {
  name: 'bpscr${suffix}'
}

resource appServicePlan 'Microsoft.Web/serverfarms@2025-03-01' = {
  name: 'bps-asp-${suffix}'
  location: location
  kind: 'linux'
  properties: {
    reserved: true
  }
  sku: {
    name: 'B1'
  }
}

resource webApp 'Microsoft.Web/sites@2025-03-01' = {
  name: 'bps-app-${suffix}'
  location: location
  tags: {}
  properties: {
    siteConfig: {
      acrUseManagedIdentityCreds: true
      alwaysOn: true
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Docker'
        }
        {
          name: 'ASPNETCORE_HTTP_PORTS'
          value: '8080'
        }
        {
          name: 'WEBSITES_PORT'
          value: '8080'
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'true'
        }
        {
          name: 'WEBSITES_CONTAINER_START_TIME_LIMIT'
          value: '230'
        }
      ]
      linuxFxVersion: 'DOCKER|${acr.properties.loginServer}/webapplication1:latest'
      http20Enabled: true
      minimumElasticInstanceCount: 1
      publicNetworkAccess: 'Enabled'
    }
    serverFarmId: appServicePlan.id
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource webAppLogConfig 'Microsoft.Web/sites/config@2022-09-01' = {
  parent: webApp
  name: 'logs'
  properties: {
    applicationLogs: {
      fileSystem: {
        level: 'Verbose'
      }
    }
    httpLogs: {
      fileSystem: {
        enabled: true
        retentionInDays: 3
        retentionInMb: 100
      }
    }
  }
}
