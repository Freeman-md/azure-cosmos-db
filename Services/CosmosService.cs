using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Newtonsoft.Json;

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

    public async Task<ItemResponse<T>> CreateItem<T>(string databaseName, string containerName, T item, string partitionKey)
    {
        Container container = GetContainer(databaseName, containerName);

        return await container.UpsertItemAsync(
            item: item,
            partitionKey: new PartitionKey(partitionKey)
        );
    }

    public async Task<ItemResponse<T>> GetItem<T>(string databaseName, string containerName, string id, string partitionKey)
    {
        Container container = GetContainer(databaseName, containerName);

        return await container.ReadItemAsync<T>(
            id,
            new PartitionKey(partitionKey)
        );
    }

    public async Task DeleteItem<T>(string databaseName, string containerName, string id, string partitionKey)
    {
        Container container = GetContainer(databaseName, containerName);

        await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
    }

    public async Task<List<T>> GetAllItems<T>(string databaseName, string containerName, string partitionKey)
    {
        Container container = GetContainer(databaseName, containerName);
        string query = "SELECT * FROM products p WHERE p.category = @category";

        var queryDef = new QueryDefinition(query)
        .WithParameter("@category", partitionKey);

        using FeedIterator<T> feed = container.GetItemQueryIterator<T>(
            queryDefinition: queryDef
        );

        List<T> items = new();
        while (feed.HasMoreResults)
        {
            FeedResponse<T> response = await feed.ReadNextAsync();
            foreach (T item in response)
            {
                items.Add(item);
            }
        }

        return items;
    }

    public async Task<StoredProcedureExecuteResponse<string>> BatchCreateItemsWithStoredProcedure(string databaseName, string containerName, string partitionKey, object[] items)
    {
        // Serialize the items array before passing it to the stored procedure
        string serializedItems = JsonConvert.SerializeObject(items);

        return await _cosmosClient
            .GetContainer(databaseName, containerName)
            .Scripts
            .ExecuteStoredProcedureAsync<string>(
                "create-products",
                new PartitionKey(partitionKey),
                new[] { serializedItems }
            );
    }

    public async Task CreateStoredProcedure(string databaseName, string containerName, string storedProcedureId)
    {
        StoredProcedureResponse storedProcedureResponse = await _cosmosClient.GetContainer(databaseName, containerName).Scripts.CreateStoredProcedureAsync(new StoredProcedureProperties
        {
            Id = storedProcedureId,
            Body = File.ReadAllText($@"..\stored-procedures\{storedProcedureId}.js")
        });
    }


}