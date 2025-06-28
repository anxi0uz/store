using Core.Models;

namespace store.application.Services;

public interface IUserService
{
    Task<IEnumerable<UserConnection>> GetAdminsAsync(CancellationToken ct);
}