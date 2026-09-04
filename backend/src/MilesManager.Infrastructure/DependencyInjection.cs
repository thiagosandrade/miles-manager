using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Infrastructure.Persistence;
using MilesManager.Infrastructure.Persistence.Repositories;
using MilesManager.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Services;

namespace MilesManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Missing connection string 'Postgres'.");

        services.AddDbContext<MilesManagerDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MilesManagerDbContext>());
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<MilesManagerDbContext>());
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
        services.AddScoped<IMileageAccountingService, MileageAccountingService>();

        return services;
    }
}
