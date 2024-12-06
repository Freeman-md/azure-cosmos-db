using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using azure_cosmos_db.Services;
using Models.Product;

IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
IConfiguration root = builder.Build();

string? cosmosDBURI = root.GetValue<string>("CosmosDB:URI");

if (string.IsNullOrEmpty(cosmosDBURI))
{
    throw new ArgumentNullException(nameof(cosmosDBURI));
}

CosmosClient client = new CosmosClient(cosmosDBURI);

static async Task CreateDatabaseAndContainer(CosmosClient client, string databaseName, string containerName, string partitionKey) {
    try
    {
        CosmosService cosmosService = new CosmosService(client);

        await cosmosService.CreateDatabase(databaseName);

        await cosmosService.CreateContainer(databaseName, containerName, partitionKey);

        Database database = cosmosService.GetDatabase(databaseName);

        Container container = cosmosService.GetContainer(databaseName, containerName);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static async Task CreateItem(CosmosClient client, string databaseName, string containerName, object item, string itemPartitionKey) {
    try
    {
        CosmosService cosmosService = new CosmosService(client);

        await cosmosService.CreateItem(databaseName, containerName, item, itemPartitionKey);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

await CreateDatabaseAndContainer(client, "TestDatabase1", "Container1", "/Category");

string category = "games";

Product product = new Product(
    id: Guid.NewGuid().ToString(),
    category: category,
    name: "New Product",
    price: 80
);

// await CreateItem(client, "TestDatabase", "Container", product, category);