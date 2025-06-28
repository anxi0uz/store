public record CreateOrderItemRequest(
    Guid OrderId,
    Guid ProductId,
    decimal Price,
    int Quantity);