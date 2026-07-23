namespace API.Auth;
using Microsoft.AspNetCore.Identity;
using Models;

public sealed class NoOpEmailSender : IEmailSender<AppUser>
{
    public Task SendConfirmationLinkAsync(AppUser user, string email, string confirmationLink)
        => Task.CompletedTask;

    public Task SendPasswordResetLinkAsync(AppUser user, string email, string resetLink)
        => Task.CompletedTask;

    public Task SendPasswordResetCodeAsync(AppUser user, string email, string resetCode)
        => Task.CompletedTask;
}