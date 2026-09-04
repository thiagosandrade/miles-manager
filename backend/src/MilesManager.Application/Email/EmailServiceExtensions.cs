using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;

namespace MilesManager.Application.Email;

public static class EmailServiceExtensions
{
    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ResendClientOptions>().Configure(options =>
        {
            options.ApiToken = configuration["Email:ApiKey"] ?? string.Empty;
        });

        services.AddSingleton<IResend, ResendClient>();
        services.AddScoped<MilesManager.Application.Abstractions.IEmailService, ResendEmailService>();
        services.AddSingleton<EmailTemplateService>();

        return services;
    }
}
