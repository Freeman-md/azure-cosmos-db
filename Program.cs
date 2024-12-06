using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
IConfiguration root = builder.Build();

string? cosmosDBURI = root.GetValue<string>("CosmosDB:URI");

if (string.IsNullOrEmpty(cosmosDBURI))
{
    throw new ArgumentNullException(nameof(cosmosDBURI));
}

CosmosClient client = new CosmosClient(cosmosDBURI, new DefaultAzureCredential());