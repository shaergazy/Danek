using Danek.DAL.Enums;
using Danek.DAL.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Danek.DAL.Seed
{
    public class DatabaseMigrator
    {
            public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<DatabaseMigrator>>();

                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var scopedProvider = scope.ServiceProvider;

                    var roleManager = scopedProvider.GetRequiredService<RoleManager<Role>>();
                    var userManager = scopedProvider.GetRequiredService<UserManager<User>>();
                    var context = scopedProvider.GetRequiredService<AppDbContext>();

                    logger.LogInformation("Starting database seeding...");

                    await SeedRolesAsync(roleManager, logger);

                    await SeedAdminUserAsync(userManager, logger);

                    await SeedAdditionalDataAsync(userManager, roleManager, logger);

                    logger.LogInformation("Database seeding completed successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the database");
                }
            }

            private static async Task SeedRolesAsync(RoleManager<Role> roleManager, ILogger logger)
            {
                try
                {
                    var roleTypes = Enum.GetValues(typeof(RoleType))
                        .Cast<RoleType>()
                        .Select(r => r.ToString())
                        .ToList();

                    foreach (var roleName in roleTypes)
                    {
                        var roleExists = await roleManager.RoleExistsAsync(roleName);

                        if (!roleExists)
                        {
                            var role = new Role
                            {
                                Name = roleName,
                                NormalizedName = roleName.ToUpperInvariant()
                            };

                            var result = await roleManager.CreateAsync(role);

                            if (result.Succeeded)
                            {
                                logger.LogInformation("Role '{RoleName}' created successfully", roleName);
                            }
                            else
                            {
                                logger.LogWarning("Failed to create role '{RoleName}': {Errors}",
                                    roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                            }
                        }
                        else
                        {
                            logger.LogDebug("Role '{RoleName}' already exists, skipping", roleName);
                        }
                    }

                    logger.LogInformation("Roles seeding completed");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error seeding roles");
                }
            }

            private static async Task SeedAdminUserAsync(UserManager<User> userManager, ILogger logger)
            {
                const string adminEmail = "admin@danek.com";
                const string adminPassword = "vrysmplpswd";
                const string adminRole = "Admin";

                try
                {
                    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

                    if (existingAdmin == null)
                    {
                        var admin = new User
                        {
                            UserName = adminEmail,
                            Email = adminEmail,
                            TelegramUsername = adminEmail,
                            EmailConfirmed = true,
                            PhoneNumber = "+10000000000",
                            PhoneNumberConfirmed = true,
                            FirstName = "System",
                            LastName = "Administrator",
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString()
                        };

                        var createResult = await userManager.CreateAsync(admin, adminPassword);

                        if (createResult.Succeeded)
                        {
                            logger.LogInformation("Admin user '{Email}' created successfully", adminEmail);

                            var addToRoleResult = await userManager.AddToRoleAsync(admin, adminRole);

                            if (addToRoleResult.Succeeded)
                            {
                                logger.LogInformation("Admin role assigned to user '{Email}'", adminEmail);
                            }
                            else
                            {
                                logger.LogWarning("Failed to assign admin role to user '{Email}': {Errors}",
                                    adminEmail, string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
                            }
                        }
                        else
                        {
                            logger.LogWarning("Failed to create admin user '{Email}': {Errors}",
                                adminEmail, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger.LogDebug("Admin user '{Email}' already exists", adminEmail);

                        var isInRole = await userManager.IsInRoleAsync(existingAdmin, adminRole);

                        if (!isInRole)
                        {
                            var addToRoleResult = await userManager.AddToRoleAsync(existingAdmin, adminRole);

                            if (addToRoleResult.Succeeded)
                            {
                                logger.LogInformation("Admin role assigned to existing user '{Email}'", adminEmail);
                            }
                            else
                            {
                                logger.LogWarning("Failed to assign admin role to existing user '{Email}': {Errors}",
                                    adminEmail, string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
                            }
                        }
                        else
                        {
                            logger.LogDebug("Admin user '{Email}' already has admin role", adminEmail);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error seeding admin user");
                }
            }

            private static async Task SeedAdditionalDataAsync(
                UserManager<User> userManager,
                RoleManager<Role> roleManager,
                ILogger logger)
            {
                try
                {
                    const string testUserEmail = "user@danek.com";
                    const string testUserPassword = "User123!";
                    const string userRole = "User";

                    var existingUser = await userManager.FindByEmailAsync(testUserEmail);

                    if (existingUser == null)
                    {
                        var testUser = new User
                        {
                            UserName = testUserEmail,
                            Email = testUserEmail,
                            TelegramUsername = testUserEmail,
                            EmailConfirmed = true,
                            FirstName = "Test",
                            LastName = "User",
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString()
                        };

                        var createResult = await userManager.CreateAsync(testUser, testUserPassword);

                        if (createResult.Succeeded)
                        {
                            logger.LogInformation("Test user '{Email}' created successfully", testUserEmail);

                            await userManager.AddToRoleAsync(testUser, userRole);
                        }
                    }


                    logger.LogInformation("Additional data seeding completed");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error seeding additional data");
                }
            }

            public static async Task ReseedDatabaseAsync(IServiceProvider serviceProvider)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<DatabaseMigrator>>();

                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var scopedProvider = scope.ServiceProvider;

                    var userManager = scopedProvider.GetRequiredService<UserManager<User>>();
                    var context = scopedProvider.GetRequiredService<AppDbContext>();

                    logger.LogWarning("Reseeding database - this will delete and recreate seed data!");

                    var seedUsers = new[] { "admin@danek.com", "user@danek.com" };

                    foreach (var email in seedUsers)
                    {
                        var user = await userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            var result = await userManager.DeleteAsync(user);
                            if (result.Succeeded)
                            {
                                logger.LogInformation("Deleted seed user '{Email}'", email);
                            }
                        }
                    }

                    await SeedDatabaseAsync(serviceProvider);

                    logger.LogInformation("Database reseeding completed");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error reseeding database");
                }
            }
        }
    }
