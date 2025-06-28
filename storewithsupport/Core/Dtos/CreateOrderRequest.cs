public record CreateOrderRequest(
    Guid UserId,
    DateTime OrderDate,
    decimal TotalAmount,
    string ShippingAddress);