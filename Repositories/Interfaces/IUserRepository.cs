using Models.Users;
using Repositories;

namespace Repositories.Interfaces;

public interface IUserRepository : IRepositoryBase<User>
{
    public Task<User?> GetUserByUsernameAsync(string username);
    public Task<bool> UserExistsAsync(string username);
    Task<bool> UserExistsAsync(Guid userId);
}