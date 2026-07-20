using System.Security.Claims;
using System.Text;
using API.Users;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace API.Auth.Services;

public class JsonWebTokenService(IConfiguration config)
{
    private DateTime GetExpirationDate()
    {
        var hours = config["JWT:Duration"];
        
        var result = int.TryParse(hours, out var hour);
        return !result ? throw new ArgumentNullException(nameof(hours)) : DateTime.Now.AddHours(hour);
    }
    
    private List<Claim> GenerateClaims(User user)
    {
        List<Claim> claims = [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
        ];

        return claims;
    }

    private SymmetricSecurityKey GenerateSecurityKey()
    {
        var secretKey = config["JWT:Secret"];
        return secretKey == null ? throw new ArgumentNullException(nameof(secretKey)) : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    }

    private SigningCredentials GenerateSigningCredentials(SymmetricSecurityKey securityKey)
    {
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    private SecurityTokenDescriptor GenerateTokenDescriptor(User user)
    {
        var claims = GenerateClaims(user);
        var securityKey = GenerateSecurityKey();
        var signingCredentials = GenerateSigningCredentials(securityKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = GetExpirationDate(),
            SigningCredentials = signingCredentials,
            Issuer = config["JWT:Issuer"],
            Audience = config["JWT:Audience"],
        };
        return tokenDescriptor;
    }

    public string GenerateToken(User user)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = GenerateTokenDescriptor(user);
        var token = handler.CreateToken(tokenDescriptor);
        return token ?? throw new Exception("Generated JWT is null!");
    }
}