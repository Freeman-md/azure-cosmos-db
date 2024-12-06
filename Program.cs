using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using azure_cosmos_db.Services;

IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
IConfiguration root = builder.Build();

string? cosmosDBURI = root.GetValue<string>("CosmosDB:URI");

if (string.IsNullOrEmpty(cosmosDBURI))
{
    throw new ArgumentNullException(nameof(cosmosDBURI));
}

CosmosClient client = new CosmosClient(cosmosDBURI, new DefaultAzureCredential());

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

await CreateDatabaseAndContainer(client, "TestDatabase1", "Container1", "/Category");