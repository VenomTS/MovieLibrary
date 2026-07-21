using API.Auth.Services;
using API.OneOfTypes;
using DTO.Users;
using Models.Users;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;

namespace API.Users;

public class UserService(HashingService hashingService, JsonWebTokenService jwtService, IUserRepository userRepository)
{
    public async Task<OneOf<Success, UserAlreadyExists>> Register(RegisterRequest request)
    {
        var userExists = await userRepository.UserExistsAsync(request.Username);
        if (userExists)
            return new UserAlreadyExists();

        hashingService.CreateHash(request.Password, out var hash, out var salt);

        var user = new User
        {
            Username = request.Username,
            Role = Roles.Customer,
            Hash = hash,
            Salt = salt,
        };

        await userRepository.CreateAsync(user);
        await userRepository.SaveChangesAsync();
        return new Success();
    }

    public async Task<OneOf<LoginResponse, InvalidLogin>> Login(LoginRequest request)
    {
        var user = await userRepository.GetUserByUsernameAsync(request.Username);
        
        // These should be unified, but I don't know how to handle that?
        if (user == null)
            return new InvalidLogin();

        var isPasswordMatch = hashingService.VerifyHash(request.Password, user.Hash, user.Salt);
        if (!isPasswordMatch)
            return new InvalidLogin();

        var token = jwtService.GenerateToken(user);
        return new LoginResponse
        {
            JsonWebToken = token,
        };
    }
}