namespace API.Users.Repository;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetUsers();
    public Task<User?> GetUserById(Guid id);
    public Task<User?> GetUserByUsername(string username);
    public Task<bool> UserExists(string username);
    public Task AddUser(User user);
}