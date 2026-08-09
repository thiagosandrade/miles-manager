using MilesManager.Application.Abstractions;
using MilesManager.Domain.Lookups;
using MilesManager.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using MilesManager.Domain.Users;
using MilesManager.Application.Features.Users.Abstractions;

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

        if (await context.Programs.AnyAsync(cancellationToken)) return;

        var programs = new[]
        {
            new MilesManager.Domain.Programs.LoyaltyProgram { Name = "Smiles", ProgramCode = "SMLS", ReferencePricePerThousand = 20m },
            new MilesManager.Domain.Programs.LoyaltyProgram { Name = "LATAM Pass", ProgramCode = "LTAM", ReferencePricePerThousand = 22m },
            new MilesManager.Domain.Programs.LoyaltyProgram { Name = "Livelo", ProgramCode = "LIV", ReferencePricePerThousand = 18m },
            new MilesManager.Domain.Programs.LoyaltyProgram { Name = "Esfera", ProgramCode = "ESF", ReferencePricePerThousand = 15m },
            new MilesManager.Domain.Programs.LoyaltyProgram { Name = "Azul Fidelidade", ProgramCode = "AZL", ReferencePricePerThousand = 19m },
            new MilesManager.Domain.Programs.LoyaltyProgram { Name = "TAP Miles&Go", ProgramCode = "TAP", ReferencePricePerThousand = 25m }
        };

        foreach (var p in programs) await context.Programs.AddAsync(p, cancellationToken);

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

        foreach (var a in airports) await context.Airports.AddAsync(a, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        var programList = await context.Programs.AsNoTracking().ToListAsync(cancellationToken);
        var referencePrices = new[]
        {
            new ReferenceMileagePrice { ProgramId = programList[0].Id, PricePerThousand = 20m, EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow) },
            new ReferenceMileagePrice { ProgramId = programList[1].Id, PricePerThousand = 22m, EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow) },
            new ReferenceMileagePrice { ProgramId = programList[2].Id, PricePerThousand = 18m, EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow) }
        };

        foreach (var rp in referencePrices) await context.ReferenceMileagePrices.AddAsync(rp, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        if (!await context.Users.AnyAsync(cancellationToken))
        {
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var user = new AppUser
            {
                FullName = "James Bond",
                Email = "jamesbond@test.com",
                PasswordHash = hasher.HashPassword("Test13245!"),
                IsActive = true
            };
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
