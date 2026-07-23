namespace DTO.Users;

public class LoginResponse
{
    public string? TokenType { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresInt { get; set; }
}