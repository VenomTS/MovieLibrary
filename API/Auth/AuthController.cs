using API.Users;
using DTO.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Auth;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(UserService userService) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var result = await userService.GetMeAsync(User);

        return result.Match<IActionResult>(
            Ok,
            _ => NotFound("AppUser not found"));
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await userService.GetUsersAsync();

        return Ok(result);
    }

    [HttpGet("roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoles()
    {
        var result = await userService.GetRolesAsync();

        return Ok(result);
    }

    [HttpPut("users/{userId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid userId, UpdateUserRoleRequest request)
    {
        var result = await userService.UpdateAsync(userId, request);
        
        return result.Match<IActionResult>(
            _ => NoContent(),
            _ => NotFound("AppUser not found"));
    }
}