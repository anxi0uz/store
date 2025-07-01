using Core.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace storewithsupport.Mappings;

public static class OrderMapping
{
    public static IEndpointRouteBuilder MapOrders(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/orders");

        group.MapGet("/", async (AppDbContext context) =>
        {
            return await context.Orders
                .Include(o => o.OrderItems)
                .Include(s=>s.User)
                .Select(s=>new OrderResponse(s.Id
                    ,s.UserId
                    ,s.User.Email
                    ,s.OrderDate
                    ,s.TotalAmount
                    ,s.ShippingAddress
                    ,s.OrderItems.Select(i=>new OrderItemResponse(
                        i.Id
                        ,i.OrderId
                        ,i.ProductId
                        ,i.Product.Name
                        ,i.Price
                        ,i.Quantity
                        ,i.Price*i.Quantity)).ToList()))
                .AsNoTracking()
                .ToListAsync();
        });

        group.MapPost("/", async (AppDbContext context, CreateOrderRequest request) =>
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                OrderDate = DateTime.Now,
                TotalAmount = request.TotalAmount,
                ShippingAddress = request.ShippingAddress
            };

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            return Results.Created($"/orders/{order.Id}", order);
        });

        group.MapPut("/{id:guid}", async (AppDbContext context, CreateOrderRequest request, Guid id) =>
        {
            var updatedRows = await context.Orders
                .Where(o => o.Id == id)
                .ExecuteUpdateAsync(p => p
                    .SetProperty(s => s.UserId, request.UserId)
                    .SetProperty(s => s.TotalAmount, request.TotalAmount)
                    .SetProperty(s => s.ShippingAddress, request.ShippingAddress));

            if (updatedRows == 0)
                return Results.NotFound(new { Message = $"Order with ID {id} not found." });

            return Results.Ok();
        });

        group.MapDelete("/{id:guid}", async (AppDbContext context, Guid id) =>
        {
            var deletedRows = await context.Orders
                .Where(o => o.Id == id)
                .ExecuteDeleteAsync();

            if (deletedRows == 0)
                return Results.NotFound(new { Message = $"Order with ID {id} not found." });

            return Results.Ok();
        });

        return endpoints;
    }
}