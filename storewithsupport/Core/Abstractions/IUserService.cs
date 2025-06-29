using Core.Models;

namespace store.application.Services;

public interface IUserService
{
    Task<Guid> RegisterUser(CancellationToken ct, CreateUserRequest request);
    Task<User?> LoginUser(string email, string password, CancellationToken ct);
}