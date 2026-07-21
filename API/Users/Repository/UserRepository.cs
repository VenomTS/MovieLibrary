using API.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Users.Repository;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<IEnumerable<User>> GetUsers()
    {
        var users = await dbContext.Users.ToListAsync();
        return users;
    }

    public async Task<User?> GetUserById(Guid id)
    {
        var user = await dbContext.Users.FindAsync(id);
        return user;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
        return user;
    }

    public async Task<bool> UserExists(string username)
    {
        var userExists = await dbContext.Users.AnyAsync(x => x.Username == username);
        return userExists;
    }

    public async Task AddUser(User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await dbContext.Users.AnyAsync(x => x.Id == userId);
    }
}