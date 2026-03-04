using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Website.Siegwart.PL.Data
{
    public static class SeedData
    {
        /// <summary>
        /// Initialize roles and an initial admin user.
        /// - Applies pending EF migrations.
        /// - Reads InitialAdmin:Email / InitialAdmin:Password / InitialAdmin:Roles from configuration (user-secrets or env).
        /// </summary>
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration, ILogger? logger = null)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            // Apply pending migrations (safe to run - will do nothing if up-to-date)
            try
            {
                using (var scopeForMigration = serviceProvider.CreateScope())
                {
                    var db = scopeForMigration.ServiceProvider.GetService<Website.Siegwart.DAL.Data.Contexts.AppDbContext>();
                    if (db != null)
                    {
                        logger?.LogInformation("Applying any pending migrations...");
                        await db.Database.MigrateAsync();
                        logger?.LogInformation("Migrations applied (if any).");
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to apply migrations before seeding. Aborting seeding.");
                return;
            }

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Read configuration values and normalize
            var adminEmailRaw = configuration["InitialAdmin:Email"] ?? configuration["AdminCredentials:Email"];
            var adminPassword = configuration["InitialAdmin:Password"] ?? configuration["AdminCredentials:Password"];
            var adminUserNameRaw = configuration["InitialAdmin:UserName"] ?? configuration["AdminCredentials:UserName"];
            var rolesCsv = configuration["InitialAdmin:Roles"] ?? configuration["AdminCredentials:Roles"] ?? "SuperAdmin,Admin";

            var adminEmail = adminEmailRaw?.Trim().ToLowerInvariant();
            var adminUserName = (adminUserNameRaw?.Trim().Length > 0) ? adminUserNameRaw.Trim() : adminEmail;

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger?.LogInformation("Initial admin credentials not configured. Skipping seeding.");
                return;
            }

            var roles = rolesCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            logger?.LogInformation("Seeding roles: {Roles}", string.Join(", ", roles));

            // Ensure roles exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        logger?.LogWarning("Failed to create role {Role}: {Errors}", role, string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                    }
                    else
                    {
                        logger?.LogInformation("Created role: {Role}", role);
                    }
                }
            }

            // Check existence by normalized email or username
            IdentityUser? existing = null;
            try
            {
                existing = await userManager.FindByEmailAsync(adminEmail);
                if (existing == null && !string.IsNullOrWhiteSpace(adminUserName))
                {
                    existing = await userManager.FindByNameAsync(adminUserName!);
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed while checking existing admin user.");
                return;
            }

            if (existing == null)
            {
                var admin = new IdentityUser
                {
                    UserName = adminUserName!,
                    Email = adminEmail!,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
                if (!createResult.Succeeded)
                {
                    logger?.LogError("Failed to create admin {Email}: {Errors}", adminEmail, string.Join("; ", createResult.Errors.Select(e => e.Description)));
                    return;
                }

                // Add roles (ignore failures individually but log them)
                foreach (var role in roles)
                {
                    var addRoleResult = await userManager.AddToRoleAsync(admin, role);
                    if (!addRoleResult.Succeeded)
                    {
                        logger?.LogWarning("Failed to add user {Email} to role {Role}: {Errors}", adminEmail, role, string.Join("; ", addRoleResult.Errors.Select(e => e.Description)));
                    }
                }

                logger?.LogInformation("Initial admin created: {Email}", adminEmail);
            }
            else
            {
                // Ensure roles assigned to existing user
                foreach (var role in roles)
                {
                    if (!await userManager.IsInRoleAsync(existing, role))
                    {
                        var addRoleResult = await userManager.AddToRoleAsync(existing, role);
                        if (!addRoleResult.Succeeded)
                        {
                            logger?.LogWarning("Failed to add existing admin {Email} to role {Role}: {Errors}", adminEmail, role, string.Join("; ", addRoleResult.Errors.Select(e => e.Description)));
                        }
                    }
                }

                logger?.LogInformation("Initial admin already exists; ensured roles.");
            }
        }
    }
}