using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Domain.Lookups;
using MilesManager.Domain.Pricing;
using MilesManager.Domain.Users;

namespace MilesManager.Api.Data;

/// <summary>
/// Seeds initial data into the database.
/// The seed is idempotent and can safely run multiple times.
/// </summary>
public static class SeedData
{
    private const string AdminEmail = "admin@admin.com";
    private const string UserEmail = "jamesbond@test.com";

    private const string UserRoleName = "User";
    private const string AdministratorRoleName = "Administrator";

    /// <summary>
    /// Initializes seed data.
    /// </summary>
    public static async Task InitializeAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<IApplicationDbContext>();

        var hasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher>();

        await SeedProgramsAsync(context, cancellationToken);
        await SeedAirportsAsync(context, cancellationToken);
        await SeedReferenceMileagePricesAsync(context, cancellationToken);

        var adminUser = await SeedUsersAsync(
            context,
            hasher,
            cancellationToken);

        var administratorRole = await SeedRolesAsync(
            context,
            cancellationToken);

        await AssignAdministratorRoleAsync(
            context,
            adminUser.Id,
            administratorRole.Id,
            cancellationToken);
    }

    private static async Task SeedProgramsAsync(
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var programs = new[]
        {
            new Domain.Programs.LoyaltyProgram
            {
                Name = "Smiles",
                ProgramCode = "SMLS",
                ReferencePricePerThousand = 20m,
                CurrencyCode = "BRL"
            },
            new Domain.Programs.LoyaltyProgram
            {
                Name = "LATAM Pass",
                ProgramCode = "LTAM",
                ReferencePricePerThousand = 22m,
                CurrencyCode = "BRL"
            },
            new Domain.Programs.LoyaltyProgram
            {
                Name = "Livelo",
                ProgramCode = "LIV",
                ReferencePricePerThousand = 18m,
                CurrencyCode = "BRL"
            },
            new Domain.Programs.LoyaltyProgram
            {
                Name = "Esfera",
                ProgramCode = "ESF",
                ReferencePricePerThousand = 15m,
                CurrencyCode = "BRL"
            },
            new Domain.Programs.LoyaltyProgram
            {
                Name = "Azul Fidelidade",
                ProgramCode = "AZL",
                ReferencePricePerThousand = 19m,
                CurrencyCode = "BRL"
            },
            new Domain.Programs.LoyaltyProgram
            {
                Name = "TAP Miles&Go",
                ProgramCode = "TAP",
                ReferencePricePerThousand = 25m,
                CurrencyCode = "BRL"
            }
        };

        foreach (var program in programs)
        {
            var exists = await context.Programs
                .AnyAsync(
                    x => x.ProgramCode == program.ProgramCode,
                    cancellationToken);

            if (!exists)
            {
                await context.Programs.AddAsync(
                    program,
                    cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedAirportsAsync(
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var airports = new[]
        {
            new Airport
            {
                IataCode = "GRU",
                Name = "São Paulo-Guarulhos"
            },
            new Airport
            {
                IataCode = "CGH",
                Name = "São Paulo-Congonhas"
            },
            new Airport
            {
                IataCode = "VCP",
                Name = "Campinas"
            },
            new Airport
            {
                IataCode = "GIG",
                Name = "Rio de Janeiro-Galeão"
            },
            new Airport
            {
                IataCode = "SDU",
                Name = "Rio de Janeiro-Santos Dumont"
            },
            new Airport
            {
                IataCode = "LIS",
                Name = "Lisbon"
            },
            new Airport
            {
                IataCode = "MAD",
                Name = "Madrid"
            },
            new Airport
            {
                IataCode = "JFK",
                Name = "New York-JFK"
            },
            new Airport
            {
                IataCode = "MIA",
                Name = "Miami"
            },
            new Airport
            {
                IataCode = "LHR",
                Name = "London-Heathrow"
            },
            new Airport
            {
                IataCode = "CDG",
                Name = "Paris-Charles de Gaulle"
            },
            new Airport
            {
                IataCode = "LUX",
                Name = "Luxembourg"
            },
            new Airport
            {
                IataCode = "BSB",
                Name = "Brasília"
            }
        };

        foreach (var airport in airports)
        {
            var exists = await context.Airports
                .AnyAsync(
                    x => x.IataCode == airport.IataCode,
                    cancellationToken);

            if (!exists)
            {
                await context.Airports.AddAsync(
                    airport,
                    cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedReferenceMileagePricesAsync(
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var referencePrices = new Dictionary<string, decimal>
        {
            ["SMLS"] = 20m,
            ["LTAM"] = 22m,
            ["LIV"] = 18m
        };

        foreach (var referencePrice in referencePrices)
        {
            var program = await context.Programs
                .FirstOrDefaultAsync(
                    x => x.ProgramCode == referencePrice.Key,
                    cancellationToken);

            if (program == null)
            {
                continue;
            }

            var effectiveDate = DateOnly.FromDateTime(DateTime.UtcNow);

            var exists = await context.ReferenceMileagePrices
                .AnyAsync(
                    x =>
                        x.ProgramId == program.Id &&
                        x.EffectiveDate == effectiveDate,
                    cancellationToken);

            if (!exists)
            {
                await context.ReferenceMileagePrices.AddAsync(
                    new ReferenceMileagePrice
                    {
                        ProgramId = program.Id,
                        PricePerThousand = referencePrice.Value,
                        EffectiveDate = effectiveDate
                    },
                    cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task<AppUser> SeedUsersAsync(
        IApplicationDbContext context,
        IPasswordHasher hasher,
        CancellationToken cancellationToken)
    {
        var adminUser = await context.Users
            .FirstOrDefaultAsync(
                x => x.Email == AdminEmail,
                cancellationToken);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                Id = Guid.NewGuid(),
                FullName = "Admin",
                Email = AdminEmail,
                PasswordHash = hasher.HashPassword("Test13245!"),
                IsActive = true
            };

            await context.Users.AddAsync(
                adminUser,
                cancellationToken);
        }

        var regularUser = await context.Users
            .FirstOrDefaultAsync(
                x => x.Email == UserEmail,
                cancellationToken);

        if (regularUser == null)
        {
            regularUser = new AppUser
            {
                Id = Guid.NewGuid(),
                FullName = "James Bond",
                Email = UserEmail,
                PasswordHash = hasher.HashPassword("Test13245!"),
                IsActive = true
            };

            await context.Users.AddAsync(
                regularUser,
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        return adminUser;
    }

    private static async Task<Role> SeedRolesAsync(
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var userRole = await context.Roles
            .FirstOrDefaultAsync(
                x => x.Name == UserRoleName,
                cancellationToken);

        if (userRole == null)
        {
            userRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = UserRoleName
            };

            await context.Roles.AddAsync(
                userRole,
                cancellationToken);
        }

        var administratorRole = await context.Roles
            .FirstOrDefaultAsync(
                x => x.Name == AdministratorRoleName,
                cancellationToken);

        if (administratorRole == null)
        {
            administratorRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = AdministratorRoleName
            };

            await context.Roles.AddAsync(
                administratorRole,
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        return administratorRole;
    }

    private static async Task AssignAdministratorRoleAsync(
        IApplicationDbContext context,
        Guid adminUserId,
        Guid administratorRoleId,
        CancellationToken cancellationToken)
    {
        var userRoleExists = await context.UserRoles
            .AnyAsync(
                x =>
                    x.UserId == adminUserId &&
                    x.RoleId == administratorRoleId,
                cancellationToken);

        if (userRoleExists)
        {
            return;
        }

        await context.UserRoles.AddAsync(
            new UserRole
            {
                UserId = adminUserId,
                RoleId = administratorRoleId
            },
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
