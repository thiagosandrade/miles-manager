using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Email;
using MilesManager.Application.Services;

namespace MilesManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddScoped<IMileageAccountingService, MileageAccountingService>();
        services.AddEmailServices(configuration);
        return services;
    }
}
