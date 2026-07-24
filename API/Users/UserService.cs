using API.OneOfTypes;
using DTO.Users;
using OneOf;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Models;

namespace API.Users;

public class UserService(UserManager<AppUser> userManager)
{
    private const string DefaultRole = "Customer";
    
    public async Task<OneOf<GetMeResponse, UserNotFound>> GetMeAsync(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            throw new Exception("JWT does not contain Id");

        var accountUser = await userManager.GetUserAsync(user);
        if(accountUser == null)
            return new UserNotFound();
        
        var roles = await userManager.GetRolesAsync(accountUser);
        if (roles.Count == 0)
        {
            await userManager.AddToRoleAsync(accountUser, DefaultRole);
            roles.Add(DefaultRole);
        }

        return new GetMeResponse
        {
            Id = accountUser.Id,
            Roles = roles,
        };
    }
}