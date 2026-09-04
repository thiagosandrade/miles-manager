using MilesManager.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Resend;

namespace MilesManager.Application.Email;

public sealed class ResendEmailService(IResend resend, IConfiguration configuration) : IEmailService
{
    private readonly IResend _resend = resend;
    private readonly string _from = configuration["Email:From"] ?? "Miles Manager <onboarding@resend.dev>";

    public async Task SendAsync(
        string recipient,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        await _resend.EmailSendAsync(
            new EmailMessage
            {
                From = _from,
                To = recipient,
                Subject = subject,
                HtmlBody = htmlBody
            },
            cancellationToken);
    }
}
