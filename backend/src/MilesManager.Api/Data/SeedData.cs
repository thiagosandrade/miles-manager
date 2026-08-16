using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Domain.Lookups;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Users;

namespace MilesManager.Api.Data;

/// <summary>
/// Seeds initial data into the database when it is empty.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Initializes seed data.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        if (!await context.Programs.AnyAsync(cancellationToken))
        {
            var programs = new[]
            {
                new Domain.Programs.LoyaltyProgram { Name = "Smiles", ProgramCode = "SMLS", ReferencePricePerThousand = 20m },
                new Domain.Programs.LoyaltyProgram { Name = "LATAM Pass", ProgramCode = "LTAM", ReferencePricePerThousand = 22m },
                new Domain.Programs.LoyaltyProgram { Name = "Livelo", ProgramCode = "LIV", ReferencePricePerThousand = 18m },
                new Domain.Programs.LoyaltyProgram { Name = "Esfera", ProgramCode = "ESF", ReferencePricePerThousand = 15m },
                new Domain.Programs.LoyaltyProgram { Name = "Azul Fidelidade", ProgramCode = "AZL", ReferencePricePerThousand = 19m },
                new Domain.Programs.LoyaltyProgram { Name = "TAP Miles&Go", ProgramCode = "TAP", ReferencePricePerThousand = 25m }
            };

            await context.Programs.AddRangeAsync(programs, cancellationToken);

            var airports = new[]
            {
                new Airport { IataCode = "GRU", Name = "São Paulo-Guarulhos" },
                new Airport { IataCode = "CGH", Name = "São Paulo-Congonhas" },
                new Airport { IataCode = "VCP", Name = "Campinas" },
                new Airport { IataCode = "GIG", Name = "Rio de Janeiro-Galeão" },
                new Airport { IataCode = "SDU", Name = "Rio de Janeiro-Santos Dumont" },
                new Airport { IataCode = "LIS", Name = "Lisbon" },
                new Airport { IataCode = "MAD", Name = "Madrid" },
                new Airport { IataCode = "JFK", Name = "New York-JFK" },
                new Airport { IataCode = "MIA", Name = "Miami" },
                new Airport { IataCode = "LHR", Name = "London-Heathrow" },
                new Airport { IataCode = "CDG", Name = "Paris-Charles de Gaulle" },
                new Airport { IataCode = "LUX", Name = "Luxembourg" },
                new Airport { IataCode = "BSB", Name = "Brasília" }
            };

            await context.Airports.AddRangeAsync(airports, cancellationToken);

            var programList = await context.Programs.AsNoTracking().ToListAsync(cancellationToken);
            var referencePrices = new[]
            {
                new ReferenceMileagePrice { ProgramId = programList[0].Id, PricePerThousand = 20m, EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow) },
                new ReferenceMileagePrice { ProgramId = programList[1].Id, PricePerThousand = 22m, EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow) },
                new ReferenceMileagePrice { ProgramId = programList[2].Id, PricePerThousand = 18m, EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow) }
            };

            await context.ReferenceMileagePrices.AddRangeAsync(referencePrices, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }


        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var adminId = new Guid();

        if (!await context.Users.AnyAsync(cancellationToken))
        {
            var users = new List<AppUser>()
            {
                new() 
                {
                    FullName = "James Bond",
                    Email = "jamesbond@test.com",
                    PasswordHash = hasher.HashPassword("Test13245!"),
                    IsActive = true
                },
                new() 
                {
                    Id = adminId,
                    FullName = "Admin",
                    Email = "admin@admin.com",
                    PasswordHash = hasher.HashPassword("Test13245!"),
                    IsActive = true
                }
            };

            await context.Users.AddRangeAsync(users, cancellationToken);
        }

        var adminRoleId = new Guid();

        if (!await context.Roles.AnyAsync(cancellationToken))
        {
            var roleList = new List<Role>()
            {
                new(){ Name = "User" },
                new(){ Id = adminRoleId, Name = "Administrator" }
            };

            await context.Roles.AddRangeAsync(roleList, cancellationToken);
        }

        if (!await context.UserRoles.AnyAsync(cancellationToken))
        {
            var userRole = new UserRole()
            {
                RoleId = adminRoleId,
                UserId = adminId
            };
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
