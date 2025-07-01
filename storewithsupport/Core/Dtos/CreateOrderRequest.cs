public record CreateOrderRequest(
    Guid UserId,
    decimal TotalAmount,
    string ShippingAddress);