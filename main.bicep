@description('Specifies the location for resources.')
param location string = 'westus'

@description('Hello world bicep storage')
resource helloStorageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'bpsstorage001'
  kind: 'StorageV2'
  location: location
  sku: { name: 'Standard_LRS' }
  properties:{
    supportsHttpsTrafficOnly: true
  }
}
