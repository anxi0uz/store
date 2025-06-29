using Core.Models;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace store.application.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _hasher;

    public UserService(AppDbContext context
        , IPasswordHasher<User> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public async Task<Guid> RegisterUser(CancellationToken ct, CreateUserRequest request)
    {
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            RoleId = request.RoleId,
            Password = _hasher.HashPassword(null, request.Password)
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task<User?> LoginUser(string email, string password, CancellationToken ct)
    {
        var user = await _context
            .Users
            .Where(p => p.Email == email)
            .FirstOrDefaultAsync(ct);
        if (user == null || _hasher.VerifyHashedPassword(user, user.Password, password) ==
            PasswordVerificationResult.Failed)
            return null;
        return user;
    }
}