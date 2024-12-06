using Microsoft.Azure.Cosmos;

namespace azure_cosmos_db.Services;

class CosmosService
{
    private readonly CosmosClient _cosmosClient;

    public CosmosService(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    public async Task<DatabaseResponse> CreateDatabase(string databaseName)
    {
        return await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
    }

    public async Task<ContainerResponse> CreateContainer(string databaseName, string containerName, string partitionKey)
    {
        Database database = GetDatabase(databaseName);

        return await database.CreateContainerIfNotExistsAsync(new ContainerProperties()
        {
            Id = containerName,
            PartitionKeyPath = partitionKey
        });
    }

    public Database GetDatabase(string databaseName)
    {
        return _cosmosClient.GetDatabase(databaseName);
    }

    public Container GetContainer(string databaseName, string containerName)
    {
        Database database = GetDatabase(databaseName);

        return database.GetContainer(containerName);
    }

    public async Task<ItemResponse<object>> CreateItem(string databaseName, string containerName, object item, string partitionKey) {
        Container container = GetContainer(databaseName, containerName);

        return await container.UpsertItemAsync(
            item: item,
            partitionKey: new PartitionKey(partitionKey)
        );
    }

}