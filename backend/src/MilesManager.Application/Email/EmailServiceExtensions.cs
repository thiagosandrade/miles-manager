using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;

namespace MilesManager.Application.Email;

public static class EmailServiceExtensions
{
    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IResend>(_ => new ResendClient(configuration["Email:ApiKey"] ?? string.Empty));
        services.AddScoped<MilesManager.Application.Abstractions.IEmailService, ResendEmailService>();
        services.AddSingleton<EmailTemplateService>();

        return services;
    }
}
