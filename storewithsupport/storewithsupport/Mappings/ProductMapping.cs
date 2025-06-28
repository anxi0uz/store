using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace storewithsupport.Mappings;

public static class ProductMapping
{
    public static IEndpointRouteBuilder MapProducts(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/products");

        group.MapGet("/", async (AppDbContext context) =>
        {
            return await context.Products.AsNoTracking().ToListAsync();
        });
        
        return endpoints;
    }
}