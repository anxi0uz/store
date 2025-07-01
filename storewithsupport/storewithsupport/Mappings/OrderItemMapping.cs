using Core.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace storewithsupport.Mappings;

public static class OrderItemMapping
{
    public static IEndpointRouteBuilder MapOrderItem(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/order-items");

        group.MapGet("/", async (AppDbContext context) =>
        {
            return await context.OrderItems.AsNoTracking().ToListAsync();
        });

        group.MapPost("/", async (AppDbContext context, CreateOrderItemRequest request) =>
        {
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = request.OrderId,
                ProductId = request.ProductId,
                Price = request.Price,
                Quantity = request.Quantity
            };

            await context.OrderItems.AddAsync(orderItem);
            await context.SaveChangesAsync();

            return Results.Created($"/order-items/{orderItem.Id}", orderItem);
        });

        group.MapPut("/{id:guid}", async (AppDbContext context, CreateOrderItemRequest request, Guid id) =>
        {
            var updatedRows = await context.OrderItems
                .Where(oi => oi.Id == id)
                .ExecuteUpdateAsync(p => p
                    .SetProperty(s => s.OrderId, request.OrderId)
                    .SetProperty(s => s.ProductId, request.ProductId)
                    .SetProperty(s => s.Price, request.Price)
                    .SetProperty(s => s.Quantity, request.Quantity));

            if (updatedRows == 0)
                return Results.NotFound(new { Message = $"OrderItem with ID {id} not found." });

            return Results.Ok();
        });

        group.MapDelete("/{id:guid}", async (AppDbContext context, Guid id) =>
        {
            var deletedRows = await context.OrderItems
                .Where(oi => oi.Id == id)
                .ExecuteDeleteAsync();

            if (deletedRows == 0)
                return Results.NotFound(new { Message = $"OrderItem with ID {id} not found." });

            return Results.Ok();
        });

        return endpoints;
    }
}