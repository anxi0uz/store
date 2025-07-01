using Core.Models;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using store.application.Services;

namespace storewithsupport.Mappings;

public static class UserMapping
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/user");

        group.MapGet("/", async (AppDbContext context) => await context.Users
            .Include(u=>u.Role)
            .Include(p=>p.Orders)
            .AsNoTracking()
            .ToListAsync());

        group.MapPost("/register", async (AppDbContext context, CreateUserRequest request,IUserService service, CancellationToken token) =>
        {
            await service.RegisterUser(token,request);
        });
        group.MapPost("/login", async (AppDbContext context, LoginRequest request, IUserService service, CancellationToken token) =>
        {
            return await service.LoginUser(request.Email,request.Password,token);
        });
        group.MapPut("/{id:guid}", async (AppDbContext context, Guid id, CreateUserRequest request, IPasswordHasher<User> passwordHasher) =>
        {
            await context.Users.Where(p => p.Id == id).ExecuteUpdateAsync(s => s
                .SetProperty(s => s.Email, request.Email)
                .SetProperty(s => s.Password, request.Password)
                .SetProperty(s => s.PhoneNumber, passwordHasher.HashPassword(null,request.Password))
                .SetProperty(s => s.Address, request.Address)
                .SetProperty(s => s.FirstName, request.FirstName)
                .SetProperty(s => s.LastName, request.LastName)
                .SetProperty(s => s.RoleId, request.RoleId));
        });

        group.MapDelete("/{id:guid}", async (AppDbContext context, Guid id) =>
        {
            await context.Users.Where(p => p.Id == id).ExecuteDeleteAsync();
            await context.SaveChangesAsync();
        });
        return routes;
    }
}