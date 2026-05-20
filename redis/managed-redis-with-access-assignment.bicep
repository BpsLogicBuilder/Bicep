@description('Location of all resources')
param location string = resourceGroup().location

@description('Name of the Redis Enterprise Cache')
param redisCacheName string = 'bps-redis-cache-${uniqueString(resourceGroup().id)}'

@description('SKU of the Redis Enterprise Cache')
param redisCacheSKU string = 'Balanced_B0'

@description('Eviction Policy of the Redis Enterprise Cache')
param evictionPolicy string = 'AllKeysLRU'

@description('Port of the Redis Enterprise Cache')
param redisPort int = 10000

@description('The Object ID of the Azure AD admin.')
param accessAssignmentPrincipalId string

resource redisEnterprise 'Microsoft.Cache/redisEnterprise@2025-08-01-preview' = {
  name: redisCacheName
  location: location
  sku: {
    name: redisCacheSKU
  }
  properties: {
    publicNetworkAccess: 'Enabled'
  }
}

resource redisDatabase 'Microsoft.Cache/redisEnterprise/databases@2025-08-01-preview' = {
  name: 'default'
  parent: redisEnterprise
  properties: {
    evictionPolicy: evictionPolicy
    clusteringPolicy: 'NoCluster'
    port: redisPort
  }
}

resource accessAssignment 'Microsoft.Cache/redisEnterprise/databases/accessPolicyAssignments@2025-08-01-preview' = {
  parent: redisDatabase
  name: 'userAccessAssignment'
  properties: {
    accessPolicyName: 'default'
    user: {
      objectId: accessAssignmentPrincipalId
    }
  }
}