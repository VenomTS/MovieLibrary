using Models.Users;
using Repositories;

namespace API.Users.Repository;

public interface IUserRepository : IRepositoryBase<User>
{
    public Task<User?> GetUserByUsernameAsync(string username);
    public Task<bool> UserExistsAsync(string username);
    Task<bool> UserExistsAsync(Guid userId);
}