using Core.Models;
using Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace storewithsupport.Mappings;

public static class RoleMapping
{
    public static IEndpointRouteBuilder MapRoles(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/roles");

        group.MapGet("/", async (AppDbContext context) => await context.Roles.AsNoTracking().ToListAsync());

        group.MapPost("/", async (AppDbContext context, RoleDTO dto) =>
        {
            var role = new Role {Id = Guid.NewGuid(), Name = dto.Name };
            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
        });
        group.MapPut("/{id:guid}", async (AppDbContext context, RoleDTO dto, Guid id) =>
        {
            await context.Roles.Where(p=>p.Id==id).ExecuteUpdateAsync(s=>s.SetProperty(s=>s.Name, dto.Name));
            await context.SaveChangesAsync();
        });
        group.MapDelete("/{id:guid}", async (AppDbContext context, Guid id) =>
        {
            await context.Roles.Where(p=>p.Id==id).ExecuteDeleteAsync();
            await context.SaveChangesAsync();
        });
        return routes;
    }
}