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