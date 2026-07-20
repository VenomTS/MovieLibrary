using API.Users;
using API.Users.DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Auth;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(UserService userService) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await userService.Register(request);

        return result.Match<IActionResult>(
            _ => NoContent(),
            _ => Conflict("Username already exists"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await userService.Login(request);

        return result.Match<IActionResult>(
            _ => Ok(result.AsT0),
            _ => Unauthorized("Username or password is incorrect"));
    }
    
}