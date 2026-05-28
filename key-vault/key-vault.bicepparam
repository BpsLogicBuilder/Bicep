using 'key-vault.bicep'

param secretsObject = {
	secrets: [
          {
            secretName: 'openai-api-key'
            secretValue: 'KeyValue1'
            contentType: 'application/x-api-key'
            tags: { 
                environment: 'development'
                service: 'openai'
            }
          }
          {
            secretName: 'cosmosdb-connection-string'
            secretValue: 'ConnectionStringValue1'
            contentType: 'application/x-connection-string'
            tags: { 
                environment: 'development'
                service: 'cosmosdb'
            }
          }
        ]
}