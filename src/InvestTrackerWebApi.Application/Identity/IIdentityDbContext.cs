namespace InvestTrackerWebApi.Application.Identity;

using System.Threading;
using System.Threading.Tasks;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRoleClaim;
using Microsoft.EntityFrameworkCore;

public interface IIdentityDbContext
{
    DbSet<ApplicationRoleClaim> RoleClaims { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
