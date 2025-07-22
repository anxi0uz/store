using Core.Dtos;
using Core.Models;

namespace store.application.Services;

public interface IUserService
{
    Task<Guid> RegisterUser(CancellationToken ct, CreateUserRequest request);
    Task<AuthResponse?> LoginUser(LoginRequest request, CancellationToken ct);
}