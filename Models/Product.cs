namespace Models.Product;

public record Product(
    string id,
    string? category,
    string name,
    decimal price,
    int? quantity = null,
    bool? clearance = null
);