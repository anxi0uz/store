public record OrderItemResponse(
    Guid Id,
    Guid OrderId,
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity,
    decimal TotalPrice);