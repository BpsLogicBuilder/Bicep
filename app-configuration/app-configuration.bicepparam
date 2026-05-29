using 'app-configuration.bicep'

param keyValuesObject = {
	items: [
          {
            key: 'OpenAI:Endpoint'
            value: 'https://my-openai.openai.azure.com/'
            contentType: null
            tags: null
          }
          {
            key: 'OpenAI:DeploymentName'
            value: 'gpt-4o'
            contentType: null
            tags: null
          }
          {
            key: 'Pipeline:BatchSize'
            value: '10'
            contentType: null
            tags: null
          }
          {
            key: 'Pipeline:RetryCount'
            value: '3'
            contentType: null
            tags: null
          }
          {
            key: 'Pipeline:BatchSize$Production'
            value: '200'
            contentType: null
            tags: null
          }
          {
            key: 'Pipeline:RetryCount$Production'
            value: '5'
            contentType: null
            tags: null
          }
          {
            key: 'Sentinel'
            value: '1'
            contentType: null
            tags: null
          }
          {
            key: 'openai-api-key'
            value: '{"uri":"https://UriToKevaultValue.com"}' /*the URI is typically an action secret*/
            contentType: 'application/vnd.microsoft.appconfig.keyvaultref+json'
            tags: null
          }
        ]
}