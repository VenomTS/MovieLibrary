namespace App.Account;

public class AppAccount
{
    public Guid Id { get; set; }
    public IList<string> Roles { get; set; } = [];
}