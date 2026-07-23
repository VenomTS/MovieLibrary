using API.Users;
using DTO.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Auth;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(UserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var result = await userService.GetMeAsync(User);

        return result.Match<IActionResult>(
            Ok,
            _ => NotFound("AppUser not found"));
    }
}