public record OrderResponse(
    Guid Id,
    Guid UserId,
    string UserEmail,
    DateTime OrderDate,
    decimal TotalAmount,
    string ShippingAddress);