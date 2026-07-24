namespace DTO.Users;

public class AppUserResponse
{
    public Guid Id { get; set; }
    public string Mail { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = [];
}