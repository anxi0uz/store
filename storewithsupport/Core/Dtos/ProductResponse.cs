public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    byte[] Image,
    int StockQuantity,
    Guid CategoryId,
    string CategoryName);