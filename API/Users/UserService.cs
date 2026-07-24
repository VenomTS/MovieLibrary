using API.OneOfTypes;
using DTO.Users;
using OneOf;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using OneOf.Types;

namespace API.Users;

public class UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
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

    public async Task<List<AppUserResponse>> GetUsersAsync()
    {
        var users = await userManager.Users.ToListAsync();

        var usersDto = new List<AppUserResponse>();
        
        foreach (var user in users)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            usersDto.Add(new AppUserResponse
            {
                Id = user.Id,
                Mail = user.Email!,
                Roles = userRoles,
            });
        }
        return usersDto;
    }

    public async Task<List<RoleResponse>> GetRolesAsync()
    {
        var roles = await roleManager.Roles.ToListAsync();
        
        return roles.Select(x => new RoleResponse
        {
            Id = x.Id,
            Name = x.Name,
        }).ToList();
    }

    public async Task<OneOf<Success, NotFound>> UpdateAsync(Guid userId, UpdateUserRoleRequest request)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return new NotFound();

        // await userManager.UpdateAsync(user);
        var userRoles = await userManager.GetRolesAsync(user);

        foreach (var role in userRoles)
        {
            var dbRole = await roleManager.FindByNameAsync(role);
            if (dbRole == null || request.Roles.Remove(dbRole.Id))
                continue;
            
            await userManager.RemoveFromRoleAsync(user, dbRole.Name!);
        }

        foreach (var role in request.Roles)
        {
            var dbRole = await roleManager.FindByIdAsync(role.ToString());
            if (dbRole == null)
                continue;
            
            await userManager.AddToRoleAsync(user, dbRole.Name!);
        }
        return new Success();
    }
}