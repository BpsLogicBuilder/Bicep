@description('Use the Resource Group Location')
param location string = resourceGroup().location

resource acr 'Microsoft.ContainerRegistry/registries@2022-12-01' = {
  name: 'bpsacr001'
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}

@description('Output the login server property for later use')
output acrLoginServer string = acr.properties.loginServer
