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
        await SeedReferenceMileagePricesAsync(context, cancellationToken);

        var adminUser = await SeedUsersAsync(context, hasher, cancellationToken);

        var administratorRole = await SeedRolesAsync(context, cancellationToken);

        await AssignAdministratorRoleAsync(context, adminUser.Id, administratorRole.Id, cancellationToken);
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
                CurrencyCode = "BRL"
            },
            new Domain.Programs.LoyaltyProgram
            {
                Name = "LATAM Pass",
                ProgramCode = "LTAM",
                CurrencyCode = "BRL"
            },
            new Domain.Programs.LoyaltyProgram
            {
                Name = "Livelo",
                ProgramCode = "LIV",
                CurrencyCode = "BRL"
            },
            new Domain.Programs.LoyaltyProgram
            {
                Name = "Esfera",
                ProgramCode = "ESF",
                CurrencyCode = "BRL"
            },
            new Domain.Programs.LoyaltyProgram
            {
                Name = "Azul Fidelidade",
                ProgramCode = "AZL",
                CurrencyCode = "BRL"
            },
            new Domain.Programs.LoyaltyProgram
            {
                Name = "TAP Miles&Go",
                ProgramCode = "TAP",
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

    private static async Task SeedReferenceMileagePricesAsync(
       IApplicationDbContext context,
       CancellationToken cancellationToken)
    {
        var referencePrices = new Dictionary<string, decimal>
        {
            ["SMLS"] = 20m,
            ["LTAM"] = 22m,
            ["LIV"] = 18m,
            ["ESF"] = 18m,
            ["AZL"] = 20m,
            ["TAP"] = 25m
        };

        var effectiveDate = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var referencePrice in referencePrices)
        {
            var program = await context.Programs
                .FirstOrDefaultAsync(
                    x => x.ProgramCode == referencePrice.Key,
                    cancellationToken);

            if (program is null)
            {
                continue;
            }

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
