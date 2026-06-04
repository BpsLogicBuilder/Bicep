@description('Specify the email address where the alerts are sent to.')
param emailAddress string = 'email@example.com'

@description('Specify the email address name where the alerts are sent to.')
param emailName string = 'oncall-email'

@description('The short name for the Action Group (max 12 characters).')
param actionGroupShortName string = 'PipeAlert'

@description('The name of the Action Group.')
param actionGroupName string = 'pipeline-alerts-ag'

@description('Location.')
param location string = resourceGroup().location

var applicationInsightsName string = 'bps-appi-${uniqueString(resourceGroup().id)}'

@description('Name of the scheduled query rule')
param alertRuleName string = 'high-failure-rate-alert'

resource applicationInsights 'Microsoft.Insights/components@2020-02-02-preview' existing = {
  name: applicationInsightsName
}

resource emailActionGroup 'microsoft.insights/actionGroups@2021-09-01' = {
  name: actionGroupName 
  location: 'global'
  properties: {
    groupShortName: actionGroupShortName
    enabled: true
    emailReceivers: [
      {
        name: emailName
        emailAddress: emailAddress
        useCommonAlertSchema: false
      }
    ]
  }
}

resource scheduledQueryRule 'Microsoft.Insights/scheduledQueryRules@2021-08-01' = {
  name: alertRuleName
  location: location
  properties: {
    description: 'Alert when more than 10 requests fail in a 5-minute window'
    enabled: true
    severity: 1
    evaluationFrequency: 'PT5M' 
    windowSize: 'PT5M'     
    scopes: [ applicationInsights.id ]
    criteria: {
      allOf: [
        {
          query: 'requests | where success == false'
          timeAggregation: 'Count'
          dimensions: []
          operator: 'GreaterThan'
          threshold: 10
          failingPeriods: {
            numberOfEvaluationPeriods: 1
            minFailingPeriodsToAlert: 1
          }
        }
      ]
    }
    actions: {
      actionGroups: [ emailActionGroup.id ]
    }
    autoMitigate: true
    checkWorkspaceAlertsStorageConfigured: false
    skipQueryValidation: false
  }
}
