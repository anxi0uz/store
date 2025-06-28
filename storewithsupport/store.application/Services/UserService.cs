using Core.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace store.application.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context) => _context = context;

    public async Task<IEnumerable<UserConnection>> GetAdminsAsync(CancellationToken ct)
    {
        return await _context.Users
            .Include(u=>u.Role)
            .Where(u => u.Role.Name == "admin")
            .Select(u => new UserConnection
            {
                UserName = u.FirstName,
                ChatRoom = "TechSupportRoom"
            })
            .ToListAsync(ct);
    }
}