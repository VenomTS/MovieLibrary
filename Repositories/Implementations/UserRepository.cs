using Microsoft.EntityFrameworkCore;
using Models.Users;
using Repositories;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class UserRepository(AppDbContext dbContext) : RepositoryBase<User>(dbContext), IUserRepository
{
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
        return user;
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        var userExists = await dbContext.Users.AnyAsync(x => x.Username == username);
        return userExists;
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await dbContext.Users.AnyAsync(x => x.Id == userId);
    }
}