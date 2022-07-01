namespace InvestTrackerWebApi.Application.Persistence;

using System.Threading;
using System.Threading.Tasks;
using InvestTrackerWebApi.Domain.Account;
using InvestTrackerWebApi.Domain.Transaction;
using Microsoft.EntityFrameworkCore;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
