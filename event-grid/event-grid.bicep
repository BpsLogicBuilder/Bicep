@description('Name of the Event Grid namespace')
param eventGridNamespaceName string = 'bps-event-grid-${uniqueString(resourceGroup().id)}'

@description('Name of the Topic')
param eventGridTopicName string = 'moderation-events'

@description('Location for all resources.')
param location string = resourceGroup().location

resource eventGridNamespace 'Microsoft.EventGrid/namespaces@2025-07-15-preview' = {
  name: eventGridNamespaceName
  location: location
  sku: {
    name: 'Standard'
    capacity: 1
  }
  properties: {}
}

resource eventGridTopic 'Microsoft.EventGrid/namespaces/topics@2025-07-15-preview' = {
  parent: eventGridNamespace
  name: eventGridTopicName
  properties: {
    publisherType: 'Custom'
    inputSchema: 'CloudEventSchemaV1_0'
    eventRetentionInDays: 1
  }
}

resource flaggedEventsTopicSubscription 'Microsoft.EventGrid/namespaces/topics/eventSubscriptions@2025-07-15-preview' = {
  parent: eventGridTopic
  name: 'sub-flagged'
  properties: {
    deliveryConfiguration: {
      deliveryMode: 'Queue'
      queue: {
        receiveLockDurationInSeconds: 60
        maxDeliveryCount: 10
        eventTimeToLive: 'P1D'
      }
    }
    eventDeliverySchema: 'CloudEventSchemaV1_0'
    filtersConfiguration: {
      includedEventTypes: [
        'com.contoso.ai.ContentFlagged'
      ]
    }
  }
}

resource approvedEventsTopicSubscription 'Microsoft.EventGrid/namespaces/topics/eventSubscriptions@2025-07-15-preview' = {
  parent: eventGridTopic
  name: 'sub-approved'
  properties: {
    deliveryConfiguration: {
      deliveryMode: 'Queue'
      queue: {
        receiveLockDurationInSeconds: 60
        maxDeliveryCount: 10
        eventTimeToLive: 'P1D'
      }
    }
    eventDeliverySchema: 'CloudEventSchemaV1_0'
    filtersConfiguration: {
      includedEventTypes: [
        'com.contoso.ai.ContentApproved'
      ]
    }
  }
}

resource allEventsTopicSubscription 'Microsoft.EventGrid/namespaces/topics/eventSubscriptions@2025-07-15-preview' = {
  parent: eventGridTopic
  name: 'sub-all-events'
  properties: {
    deliveryConfiguration: {
      deliveryMode: 'Queue'
      queue: {
        receiveLockDurationInSeconds: 60
        maxDeliveryCount: 10
        eventTimeToLive: 'P1D'
      }
    }
    eventDeliverySchema: 'CloudEventSchemaV1_0'
  }
}