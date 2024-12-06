using Newtonsoft.Json;

namespace Models.Product;

public class Product
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("price")]
    public double Price { get; set; }

    public Product(string id, string category, string name, double price)
    {
        Id = id;
        Category = category;
        Name = name;
        Price = price;
    }
}
