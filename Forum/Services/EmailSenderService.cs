using Microsoft.AspNetCore.Identity.UI.Services;

namespace NETForum.Services;

public class EmailSenderService : IEmailSender
{
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(ILogger<EmailSenderService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        return Task.CompletedTask;
    }
}