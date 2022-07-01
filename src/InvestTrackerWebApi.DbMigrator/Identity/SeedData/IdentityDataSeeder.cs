namespace InvestTrackerWebApi.DbMigrator.Identity.SeedData;

using Bogus;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

public class IdentityDataSeeder
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<IdentityDataSeeder> logger;

    public IdentityDataSeeder(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<IdentityDataSeeder> logger)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
        this.logger = logger;
    }

    public async Task SeedDatabaseAsync()
    {
        await this.SeedRootAdminUserAsync();
        await this.SeedDefaultRolesAsync();
        await this.SeedRootAdminUserRolesAsync();
        await this.SeedAdminUserAsync();
        await this.SeedManagerUserAsync();
        await this.SeedBasicUserAsync();
        await this.SeedTestRolesAsync();
        await this.SeedTestUsersAsync();
    }

    private async Task SeedRootAdminUserAsync()
    {
        if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == RootConstants.RootAdminUserEmail) is not ApplicationUser rootAdminUser)
        {
            string rootAdminUserName = RootConstants.RootAdminUserName;
            rootAdminUser = new ApplicationUser(RootConstants.RootAdminUserEmail, RootConstants.RootAdminUserFirstName, RootConstants.RootAdminUserLastName, string.Empty, rootAdminUserName, new Faker().Phone.PhoneNumber(), Guid.Empty);
            rootAdminUser = rootAdminUser.ToggleUserStatus(true, Guid.Empty);
            rootAdminUser = rootAdminUser.ConfirmEmail(Guid.Empty);
            this.logger.LogInformation("Seeding Root Admin User.");
            var password = new PasswordHasher<ApplicationUser>();
            rootAdminUser.PasswordHash = password.HashPassword(rootAdminUser, "rootadmin");
            _ = await this.userManager.CreateAsync(rootAdminUser);
        }
    }

    private async Task SeedDefaultRolesAsync()
    {
        if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == RootConstants.RootAdminUserEmail) is ApplicationUser rootAdminUser)
        {
            foreach (string roleName in DefaultRoles.Get())
            {
                if (await this.roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName) is not ApplicationRole role)
                {
                    // Create the role
                    role = new ApplicationRole(roleName, $"{roleName}", Guid.Parse(rootAdminUser.Id));
                    this.logger.LogInformation("Seeding {role} Role", roleName);
                    _ = await this.roleManager.CreateAsync(role);
                }

                // Assign permissions
                if (roleName == DefaultRoles.RootAdmin)
                {
                    await this.AssignPermissionsToRoleAsync(role, Permissions.RootAdmin.Select(x => x.Name).ToList());
                }
                else if (roleName == DefaultRoles.Admin)
                {
                    await this.AssignPermissionsToRoleAsync(role, Permissions.Admin.Select(x => x.Name).ToList());
                }
                else if (roleName == DefaultRoles.Manager)
                {
                    await this.AssignPermissionsToRoleAsync(role, Permissions.Manager.Select(x => x.Name).ToList());
                }
                else if (roleName == DefaultRoles.Basic)
                {
                    await this.AssignPermissionsToRoleAsync(role, Permissions.Basic.Select(x => x.Name).ToList());
                }
            }
        }
    }

    private async Task SeedTestRolesAsync()
    {
        if (await this.roleManager.Roles.CountAsync() < 10)
        {
            for (int i = 0; i < 100; i++)
            {
                if (new Random().NextDouble() < 50.0 / 100.0)
                {
                    if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == RootConstants.RootAdminUserEmail) is ApplicationUser rootAdminUser)
                    {

                        var roleName = new Faker().Lorem.Word();
                        if (await this.roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName) is not ApplicationRole role)
                        {
                            role = new ApplicationRole(roleName, $"{roleName} {new Faker().Lorem.Word()}", Guid.Parse(rootAdminUser.Id));
                            this.logger.LogInformation("Seeding {role} Role", roleName);
                            _ = await this.roleManager.CreateAsync(role);

                            await this.AssignPermissionsToRoleAsync(role, Permissions.RootAdmin.OrderBy(x => Guid.NewGuid()).Take(new Random().Next(0, 15)).Select(x => x.Name).ToList());
                        }
                    }
                }
                else
                {
                    if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == "admin@InvestTracker.com") is ApplicationUser adminUser)
                    {
                        var roleName = new Faker().Lorem.Word();
                        if (await this.roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName) is not ApplicationRole role)
                        {
                            role = new ApplicationRole(roleName, $"{roleName} {new Faker().Lorem.Word()}", Guid.Parse(adminUser.Id));
                            this.logger.LogInformation("Seeding {role} Role", roleName);
                            _ = await this.roleManager.CreateAsync(role);

                            await this.AssignPermissionsToRoleAsync(role, Permissions.RootAdmin.OrderBy(x => Guid.NewGuid()).Take(new Random().Next(0, 15)).Select(x => x.Name).ToList());
                        }
                    }
                }
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationRole role, List<string> permissions)
    {
        var currentClaims = await this.roleManager.GetClaimsAsync(role);
        foreach (string permission in permissions)
        {
            if (!currentClaims.Any(a => a.Type == Domain.Identity.ClaimTypes.Permission
            && a.Value == permission))
            {
                this.logger.LogInformation(
                    "Seeding {role} Permission '{permission}'",
                    role.Name,
                    permission);
                var claim = new Claim(Domain.Identity.ClaimTypes.Permission, permission);
                _ = await this.roleManager.AddClaimAsync(role, claim);
            }
        }
    }

    private async Task SeedRootAdminUserRolesAsync()
    {
        if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == RootConstants.RootAdminUserEmail) is ApplicationUser rootAdminUser)
        {
            // Assign role to user
            if (!await this.userManager.IsInRoleAsync(rootAdminUser, DefaultRoles.RootAdmin))
            {
                this.logger.LogInformation("Assigning Root Admin Role to Root Admin User.");
                _ = await this.userManager.AddToRoleAsync(rootAdminUser, DefaultRoles.RootAdmin);
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == RootConstants.RootAdminUserEmail) is ApplicationUser rootAdminUser)
        {
            if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == "admin@InvestTracker.com") is not ApplicationUser adminUser)
            {
                string adminUserName = $"{DefaultRoles.Admin}".ToLowerInvariant();
                adminUser = new ApplicationUser("admin@InvestTracker.com", "Administrator", "InvestTracker", string.Empty, adminUserName, new Faker().Phone.PhoneNumber(), Guid.Parse(rootAdminUser.Id));
                adminUser = adminUser.ToggleUserStatus(true, Guid.Parse(rootAdminUser.Id));
                adminUser = adminUser.ConfirmEmail(Guid.Parse(rootAdminUser.Id));
                this.logger.LogInformation("Seeding Admin User.");
                var password = new PasswordHasher<ApplicationUser>();
                adminUser.PasswordHash = password.HashPassword(adminUser, "admin");
                _ = await this.userManager.CreateAsync(adminUser);
            }

            // Assign role to user
            if (!await this.userManager.IsInRoleAsync(adminUser, DefaultRoles.Admin))
            {
                this.logger.LogInformation("Assigning Admin Role to Admin User.");
                _ = await this.userManager.AddToRoleAsync(adminUser, DefaultRoles.Admin);
            }
        }

    }

    private async Task SeedManagerUserAsync()
    {
        if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == RootConstants.RootAdminUserEmail) is ApplicationUser rootAdminUser)
        {
            if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == "manager@InvestTracker.com") is not ApplicationUser managerUser)
            {
                string managerUserName = $"{DefaultRoles.Manager}".ToLowerInvariant();
                managerUser = new ApplicationUser("manager@InvestTracker.com", "Manager", "InvestTracker", string.Empty, managerUserName, new Faker().Phone.PhoneNumber(), Guid.Parse(rootAdminUser.Id));
                managerUser = managerUser.ToggleUserStatus(true, Guid.Parse(rootAdminUser.Id));
                managerUser = managerUser.ConfirmEmail(Guid.Parse(rootAdminUser.Id));
                this.logger.LogInformation("Seeding Manager User.");
                var password = new PasswordHasher<ApplicationUser>();
                managerUser.PasswordHash = password.HashPassword(managerUser, "manager");
                _ = await this.userManager.CreateAsync(managerUser);
            }

            // Assign role to user
            if (!await this.userManager.IsInRoleAsync(managerUser, DefaultRoles.Manager))
            {
                this.logger.LogInformation("Assigning Manager Role to Manager User.");
                _ = await this.userManager.AddToRoleAsync(managerUser, DefaultRoles.Manager);
            }
        }

    }

    private async Task SeedBasicUserAsync()
    {
        if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == RootConstants.RootAdminUserEmail) is ApplicationUser rootAdminUser)
        {
            if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == "basic@InvestTracker.com") is not ApplicationUser basicUser)
            {
                string basicUserName = $"{DefaultRoles.Basic}".ToLowerInvariant();
                basicUser = new ApplicationUser("basic@InvestTracker.com", "Basic", "InvestTracker", "manager@InvestTracker.com", basicUserName, new Faker().Phone.PhoneNumber(), Guid.Parse(rootAdminUser.Id));
                basicUser = basicUser.ToggleUserStatus(true, Guid.Parse(rootAdminUser.Id));
                basicUser = basicUser.ConfirmEmail(Guid.Parse(rootAdminUser.Id));
                this.logger.LogInformation("Seeding Basic User.");
                var password = new PasswordHasher<ApplicationUser>();
                basicUser.PasswordHash = password.HashPassword(basicUser, "basic");
                _ = await this.userManager.CreateAsync(basicUser);
            }

            // Assign role to user
            if (!await this.userManager.IsInRoleAsync(basicUser, DefaultRoles.Basic))
            {
                this.logger.LogInformation("Assigning Basic Role to Basic User.");
                _ = await this.userManager.AddToRoleAsync(basicUser, DefaultRoles.Basic);
            }
        }

    }

    private async Task SeedTestUsersAsync()
    {
        if (await this.userManager.Users.CountAsync() < 10)
        {
            for (int userCount = 0; userCount < 100; userCount++)
            {
                if (new Random().NextDouble() < 50.0 / 100.0)
                {
                    if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == RootConstants.RootAdminUserEmail) is ApplicationUser rootAdminUser)
                    {

                        var faker = new Faker();
                        var gender = faker.PickRandom<Bogus.DataSets.Name.Gender>();
                        var firstName = faker.Name.FirstName(gender);
                        var lastName = faker.Name.LastName(gender);
                        var userName = faker.Internet.UserName(firstName, lastName);
                        var email = faker.Internet.Email(firstName, lastName);

                        if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == email) is not ApplicationUser testUser)
                        {
                            testUser = new ApplicationUser(email, firstName, lastName, string.Empty, userName, new Faker().Phone.PhoneNumber(), Guid.Parse(rootAdminUser.Id));
                            testUser = testUser.ToggleUserStatus(new Random().NextDouble() < 50.0 / 100.0, Guid.Parse(rootAdminUser.Id));

                            if (new Random().NextDouble() < 50.0 / 100.0)
                            {
                                testUser = testUser.ConfirmEmail(Guid.Parse(rootAdminUser.Id));
                            }

                            this.logger.LogInformation("Seeding Test User.");
                            var password = new PasswordHasher<ApplicationUser>();
                            testUser.PasswordHash = password.HashPassword(testUser, userName);
                            _ = await this.userManager.CreateAsync(testUser);
                        }

                        // Assign random roles to test user
                        for (int roleCount = 0; roleCount < new Random().Next(0, 5); roleCount++)
                        {
                            var roles = await this.roleManager.Roles.ToListAsync();
                            var role = roles.OrderBy(x => Guid.NewGuid()).Take(1).First();
                            if (!await this.userManager.IsInRoleAsync(testUser, role?.Name))
                            {
                                this.logger.LogInformation($"Assigning {role?.Name} Role");
                                _ = await this.userManager.AddToRoleAsync(testUser, role?.Name);
                            }
                        }

                        // Assign basic role to user
                        if (!await this.userManager.IsInRoleAsync(testUser, DefaultRoles.Basic))
                        {
                            this.logger.LogInformation("Assigning Basic Role to User.");
                            _ = await this.userManager.AddToRoleAsync(testUser, DefaultRoles.Basic);
                        }
                    }
                }
                else
                {
                    if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == "admin@InvestTracker.com") is ApplicationUser adminUser)
                    {
                        var referralUserEmails = this.userManager.Users.Select(x => x.ReferralEmail).ToList();
                        var referralUserEmail = referralUserEmails.OrderBy(x => Guid.NewGuid()).Take(1).First();
                        var faker = new Faker();
                        var gender = faker.PickRandom<Bogus.DataSets.Name.Gender>();
                        var firstName = faker.Name.FirstName(gender);
                        var lastName = faker.Name.LastName(gender);
                        var userName = faker.Internet.UserName(firstName, lastName);
                        var email = faker.Internet.Email(firstName, lastName);

                        if (await this.userManager.Users.FirstOrDefaultAsync(u => u.Email == email) is not ApplicationUser testUser)
                        {
                            testUser = new ApplicationUser(email, firstName, lastName, referralUserEmail, userName, new Faker().Phone.PhoneNumber(), Guid.Parse(adminUser.Id));
                            testUser = testUser.ToggleUserStatus(new Random().NextDouble() < 50.0 / 100.0, Guid.Parse(adminUser.Id));

                            if (new Random().NextDouble() < 50.0 / 100.0)
                            {
                                testUser = testUser.ConfirmEmail(Guid.Parse(adminUser.Id));
                            }

                            this.logger.LogInformation("Seeding Test User.");
                            var password = new PasswordHasher<ApplicationUser>();
                            testUser.PasswordHash = password.HashPassword(testUser, userName);
                            _ = await this.userManager.CreateAsync(testUser);
                        }

                        // Assign random roles to test user
                        for (int roleCount = 0; roleCount < new Random().Next(0, 5); roleCount++)
                        {
                            var roles = await this.roleManager.Roles.ToListAsync();
                            var role = roles.OrderBy(x => Guid.NewGuid()).Take(1).First();
                            if (!await this.userManager.IsInRoleAsync(testUser, role?.Name))
                            {
                                this.logger.LogInformation($"Assigning {role?.Name} Role");
                                _ = await this.userManager.AddToRoleAsync(testUser, role?.Name);
                            }
                        }

                        // Assign basic role to user
                        if (!await this.userManager.IsInRoleAsync(testUser, DefaultRoles.Basic))
                        {
                            this.logger.LogInformation("Assigning Basic Role to User.");
                            _ = await this.userManager.AddToRoleAsync(testUser, DefaultRoles.Basic);
                        }
                    }
                }
            }
        }
    }
}
