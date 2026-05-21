@description('Name of the Service Bus namespace')
param serviceBusNamespaceName string = 'bps-service-bus-${uniqueString(resourceGroup().id)}'

@description('Name of the Queue')
param serviceBusQueueName string = 'inference-requests'

@description('Name of the Topic')
param serviceBusTopicName string = 'inference-results'

@description('Location for all resources.')
param location string = resourceGroup().location

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2025-05-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {}
}

resource serviceBusQueue 'Microsoft.ServiceBus/namespaces/queues@2025-05-01-preview' = {
  parent: serviceBusNamespace
  name: serviceBusQueueName
  properties: {
    deadLetteringOnMessageExpiration: true
    maxDeliveryCount: 5
  }
}

resource serviceBusTopic 'Microsoft.ServiceBus/namespaces/topics@2025-05-01-preview' = {
  parent: serviceBusNamespace
  name: serviceBusTopicName
  properties: {}
}

resource notificationsSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2025-05-01-preview' = {
  parent: serviceBusTopic
  name: 'notifications'
  properties: {}
}

resource highPrioritySubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2025-05-01-preview' = {
  parent: serviceBusTopic
  name: 'high-priority'
  properties: {}
}

resource highPrioritySubscriptionFilter 'Microsoft.ServiceBus/namespaces/topics/subscriptions/rules@2025-05-01-preview' = {
  parent: highPrioritySubscription
  name: 'high-priority-filter'
  properties: {
    action: {}
    filterType: 'SqlFilter'
    sqlFilter: {
      sqlExpression: 'priority = \'high\''
    }
  }
}