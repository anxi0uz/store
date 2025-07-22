using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Dtos;
using Core.Models;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace store.application.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IConfiguration _configuration;

    public UserService(AppDbContext context
        , IPasswordHasher<User> hasher
        , IConfiguration configuration)
    {
        _context = context;
        _hasher = hasher;
        _configuration = configuration;
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

    public async Task<AuthResponse?> LoginUser(LoginRequest request, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(r => r.Email == request.email);
        var result = _hasher.VerifyHashedPassword(user, user.Password, request.password);
        if (result == PasswordVerificationResult.Failed)
            return null;
        var token = GenerateToken(user);
        return new AuthResponse(token, DateTime.Now.AddHours(1));
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName ?? user.Email),
        };
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}