public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    byte[] Image,
    int StockQuantity,
    Guid CategoryId);