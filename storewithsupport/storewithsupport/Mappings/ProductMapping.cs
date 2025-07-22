using Core.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace storewithsupport.Mappings;

public static class ProductMapping
{
    public static IEndpointRouteBuilder MapProducts(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/products").RequireAuthorization();

        group.MapGet("/", async (AppDbContext context) => { return await context.Products.AsNoTracking().ToListAsync(); });

        group.MapPost("/", async (AppDbContext context, CreateProductRequest request) =>
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId
            };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
        });

        group.MapPut("/{id:guid}", async (AppDbContext context, CreateProductRequest request, Guid productId) =>
        {
            var product = await context.Products.Where(p=>p.Id == productId).FirstOrDefaultAsync();
            if (product == null)
                throw new NullReferenceException($"Product with id {productId} not found");
            else
                await context.Products
                    .Where(p=>p.Id==productId)
                    .ExecuteUpdateAsync(p => p
                    .SetProperty(s => s.Name, request.Name)
                    .SetProperty(s => s.Price, request.Price)
                    .SetProperty(s => s.Description, request.Description)
                    .SetProperty(s => s.StockQuantity, request.StockQuantity)
                    .SetProperty(s => s.CategoryId, request.CategoryId));
        });

        group.MapDelete("/{id:guid}", async (AppDbContext context, Guid productId) =>
        {
            var product = await context.Products.Where(p=>p.Id == productId).FirstOrDefaultAsync();
            if (product == null)
                throw new NullReferenceException($"Product with id {productId} not found");
            else
                await context.Products.Where(p => p.Id == productId)
                    .ExecuteDeleteAsync();
        });

        return endpoints;
    }
}