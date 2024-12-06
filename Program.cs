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

static async Task CreateDatabaseAndContainer(CosmosClient client, string databaseName, string containerName, string partitionKey)
{
    CosmosService cosmosService = new CosmosService(client);

    await cosmosService.CreateDatabase(databaseName);

    await cosmosService.CreateContainer(databaseName, containerName, partitionKey);

    Database database = cosmosService.GetDatabase(databaseName);

    Container container = cosmosService.GetContainer(databaseName, containerName);
}

static async Task<ItemResponse<T>> CreateItem<T>(CosmosClient client, string databaseName, string containerName, T item, string itemPartitionKey)
{
    CosmosService cosmosService = new CosmosService(client);

    return await cosmosService.CreateItem<T>(databaseName, containerName, item, itemPartitionKey);
}

static async Task<ItemResponse<T>> GetItem<T>(CosmosClient client, string databaseName, string containerName, string id, string partitionKey)
{
    CosmosService cosmosService = new CosmosService(client);

    return await cosmosService.GetItem<T>(databaseName, containerName, id, partitionKey);
}

static async Task DeleteItem<T>(CosmosClient client, string databaseName, string containerName, string id, string partitionKey)
{
    CosmosService cosmosService = new CosmosService(client);

    await cosmosService.DeleteItem<T>(databaseName, containerName, id, partitionKey);
}

static async Task<List<T>> GetAllItems<T>(CosmosClient client, string databaseName, string containerName, string partitionKey)
{
    CosmosService cosmosService = new CosmosService(client);

    return await cosmosService.GetAllItems<T>(databaseName, containerName, partitionKey);
}

// try
// {
CosmosService cosmosService = new CosmosService(client);

await CreateDatabaseAndContainer(client, "TestDatabase1", "Container1", "/Category");

string category = "games";
Product[] products = new Product[20];

for (int i = 0; i < 20; i++)
{
    string id = Guid.NewGuid().ToString();
    products[i] = new Product(
        id: id,
        category: category,
        name: "New Product " + id,
        price: 80
    );
}

await cosmosService.BatchCreateItemsWithStoredProcedure("TestDatabase", "Container", category, products);


// Product retrievedProduct = await GetItem<Product>(client, "TestDatabase", "Container", createdProduct.id, createdProduct.category);

// Console.WriteLine(retrievedProduct);

// await DeleteItem<Product>(client, "TestDatabase", "Container", "d043df90-ab11-483b-abb6-b28424fa5a40", "games");
// List<Product> products = await GetAllItems<Product>(client, "TestDatabase", "Container", "games");

// foreach (var item in products) {
//     Console.WriteLine(item);
//     // }
// }
// catch (Exception ex)
// {
//     Console.WriteLine(ex.Message);
// }