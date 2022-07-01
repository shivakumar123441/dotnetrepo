namespace InvestTrackerWebApi.DbMigrator.Application.SeedData;

using Bogus;
using InvestTrackerWebApi.Application.Persistence;
using InvestTrackerWebApi.Domain.Account;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using InvestTrackerWebApi.Domain.Transaction;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class InvestmentsDataSeeder
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IApplicationDbContext applicationDbContext;
    private readonly ILogger<InvestmentsDataSeeder> logger;

    public InvestmentsDataSeeder(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext applicationDbContext,
        ILogger<InvestmentsDataSeeder> logger)
    {
        this.userManager = userManager;
        this.applicationDbContext = applicationDbContext;
        this.logger = logger;
    }

    public async Task SeedDatabaseAsync()
    {
        await this.SeedTestAccountsAsync();
        //await this.SeedTestTransactionsAsync();
    }

    private async Task SeedTestAccountsAsync()
    {
        int accountIncrementer = 0;
        if (await this.applicationDbContext.Accounts.CountAsync() < 10)
        {
            foreach (var userId in this.userManager.Users.Select(x => x.Id).ToList())
            {
                // Wallet Account
                if (new Random().NextDouble() < 50.0 / 100.0)
                {
                    var manager = this.userManager.Users.First(u => u.Email == "manager@InvestTracker.com");
                    var accountDescription = new Faker().Lorem.Word();
                    var accountName = "WA" + string.Format("{0:00000000}", accountIncrementer + 1);
                    var account = new Account(Guid.NewGuid(), accountName, accountDescription, AccountType.Wallet, AccountStatus.Created, Guid.Parse(userId), Guid.Parse(manager.Id));

                    this.logger.LogInformation("Seeding {accountName} Account", accountName);
                    await this.applicationDbContext.Accounts.AddAsync(account);
                }
                else
                {
                    var accountDescription = new Faker().Lorem.Word();
                    var accountName = "WA" + string.Format("{0:00000000}", accountIncrementer + 1);
                    var account = new Account(Guid.NewGuid(), accountName, accountDescription, AccountType.Wallet, AccountStatus.Created, Guid.Parse(userId), Guid.Parse(userId));

                    this.logger.LogInformation("Seeding {accountName} Account", accountName);
                    await this.applicationDbContext.Accounts.AddAsync(account);
                }

                // Referral Accounts
                var referralEmail = this.userManager.Users.Where(x => x.Id == userId).First().ReferralEmail;

                if (!string.IsNullOrEmpty(referralEmail))
                {
                    var referralUser = this.userManager.Users.Where(x => x.Email == referralEmail).First();

                    var accountDescription = new Faker().Lorem.Word();
                    var accountName = "RE" + string.Format("{0:00000000}", accountIncrementer + 1);
                    var account = new Account(Guid.NewGuid(), accountName, accountDescription, AccountType.Referral, AccountStatus.Created, Guid.Parse(referralUser.Id), Guid.Parse(userId));

                    this.logger.LogInformation("Seeding {accountName} Account", accountName);
                    await this.applicationDbContext.Accounts.AddAsync(account);
                }

                // Fixed Deposit Accounts
                if (new Random().NextDouble() < 50.0 / 100.0)
                {
                    for (int i = 0; i < new Random().Next(0, 6); i++)
                    {
                        if (new Random().NextDouble() < 50.0 / 100.0)
                        {
                            var manager = this.userManager.Users.First(u => u.Email == "manager@InvestTracker.com");
                            var accountDescription = new Faker().Lorem.Word();
                            var accountName = "FD" + string.Format("{0:00000000}", accountIncrementer + 1);
                            var account = new Account(Guid.NewGuid(), accountName, accountDescription, AccountType.FixedDeposit, AccountStatus.Created, Guid.Parse(userId), Guid.Parse(manager.Id));

                            this.logger.LogInformation("Seeding {accountName} Account", accountName);
                            await this.applicationDbContext.Accounts.AddAsync(account);
                        }
                        else
                        {
                            var accountDescription = new Faker().Lorem.Word();
                            var accountName = "FD" + string.Format("{0:00000000}", accountIncrementer + 1);
                            var account = new Account(Guid.NewGuid(), accountName, accountDescription, AccountType.FixedDeposit, AccountStatus.Created, Guid.Parse(userId), Guid.Parse(userId));

                            this.logger.LogInformation("Seeding {accountName} Account", accountName);
                            await this.applicationDbContext.Accounts.AddAsync(account);
                        }

                        accountIncrementer++;
                    }
                }

                accountIncrementer++;

                await this.applicationDbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedTestTransactionsAsync()
    {
        if (await this.applicationDbContext.Transactions.CountAsync() < 10)
        {
            foreach (var accountId in this.applicationDbContext.Accounts.Select(x => x.Id).ToList())
            {
                for (int i = 0; i < new Random().Next(0, 11); i++)
                {
                    if (new Random().NextDouble() < 50.0 / 100.0)
                    {
                        var manager = this.userManager.Users.First(u => u.Email == "manager@InvestTracker.com");
                        var amount = new Random().NextInt64(125, 4526);
                        var refCode = new Faker().Lorem.Word();
                        var transaction = new Transaction(Guid.NewGuid(), accountId, refCode, TransactionType.Deposit, amount, $"UserComments {refCode}", $"TransComments {refCode}", DateTime.UtcNow, Guid.Parse(manager.Id));
                        this.logger.LogInformation("Seeding {transactionName} Transaction", refCode);
                        await this.applicationDbContext.Transactions.AddAsync(transaction);
                    }
                    else
                    {
                        var userId = this.applicationDbContext.Accounts.Where(x => x.Id == accountId).Select(x => x.UserId).First();
                        var amount = new Random().NextInt64(125, 4526);
                        var refCode = new Faker().Lorem.Word();
                        var transaction = new Transaction(Guid.NewGuid(), accountId, refCode, TransactionType.Deposit, amount, $"UserComments {refCode}", $"TransComments {refCode}", DateTime.UtcNow, userId);
                        this.logger.LogInformation("Seeding {transactionName} Transaction", refCode);
                        await this.applicationDbContext.Transactions.AddAsync(transaction);

                    }
                }

                await this.applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
