@description('Specifies the name of the app configuration store.')
param configStoreName string = 'bps-appconfig-${uniqueString(resourceGroup().id)}'

@description('Specifies the Azure location where the app configuration store should be created.')
param location string = resourceGroup().location

@description('Specifies the SKU of the app configuration store.')
param skuName string = 'standard'

@description('Specifies all new values {"key":"","value":""} wrapped in a secure object.')
@secure()
param keyValuesObject object

resource configStore 'Microsoft.AppConfiguration/configurationStores@2024-05-01' = {
  name: configStoreName
  location: location
  sku: {
    name: skuName
  }
}

resource keyValues 'Microsoft.AppConfiguration/configurationStores/keyValues@2024-05-01' = [for kvp in keyValuesObject.items: {
  name: kvp.key
  parent: configStore
  properties: {
    value: kvp.value
    contentType: kvp.contentType
    tags: kvp.tags
  }
}]