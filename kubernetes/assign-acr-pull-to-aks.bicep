@description('Generate a Suffix based on the Resource Group ID')
param suffix string = uniqueString(resourceGroup().id)

@description('Set the ACR Pull Role Definition ID')
param acrPullRoleDefinitionID string = '7f951dda-4ed3-4680-a7ca-43fe172d538d'

@description('Generate a unique GUID to use as name for the role assignment')
var aksToAcrRoleAssignmentName = guid(aks.id, acrPullRoleDefinitionID, acr.id)

@description('Specifies the name of the container app.')
param aksClusterName string = 'bps-aks-${uniqueString(resourceGroup().id)}'

resource acr 'Microsoft.ContainerRegistry/registries@2025-11-01' existing = {
  name: 'bpscr${suffix}'
}

resource aks 'Microsoft.ContainerService/managedClusters@2024-02-01' existing = {
  name: aksClusterName
}

resource containerAppToAcrRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: acr
  name: aksToAcrRoleAssignmentName 
  properties: {
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', acrPullRoleDefinitionID)
    principalId: aks.properties.identityProfile.kubeletidentity.objectId
    principalType: 'ServicePrincipal'
  }
}