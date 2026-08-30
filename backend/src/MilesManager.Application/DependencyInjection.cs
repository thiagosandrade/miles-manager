using Microsoft.Extensions.DependencyInjection;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;

namespace MilesManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddScoped<IMileageAccountingService, MileageAccountingService>();
        return services;
    }
}
