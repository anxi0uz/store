using Core.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace storewithsupport.Mappings;

public static class CategoryMapping
{
    public static IEndpointRouteBuilder MapCategory(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/category");

        group.MapPost("/", async (AppDbContext context, CreateCategoryRequest request) =>
        {
            var category = new Category()
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                Name = request.Name,
            };
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
        });
        group.MapGet("/", async (AppDbContext context) =>
        {
            await context.Categories.AsNoTracking().ToListAsync();
        });
        group.MapPut("/{id:guid}", async (AppDbContext context, CreateCategoryRequest request, Guid id) =>
        {
            await context.Categories.Where(p => p.Id == id)
                .ExecuteUpdateAsync(s=>s
                    .SetProperty(s=>s.Description,request.Description)
                    .SetProperty(s=>s.Name,request.Name));
        });
        group.MapDelete("/{id:guid}", async (AppDbContext context, Guid id) =>
        {
            await context.Categories.Where(p => p.Id == id)
                .ExecuteDeleteAsync();
        });
        return endpoints;
    }
}