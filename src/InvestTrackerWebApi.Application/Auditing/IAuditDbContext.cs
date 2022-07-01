namespace InvestTrackerWebApi.Application.Auditing;

using System.Threading;
using System.Threading.Tasks;
using InvestTrackerWebApi.Domain.Audit;
using Microsoft.EntityFrameworkCore;

public interface IAuditDbContext
{
    DbSet<Trail> AuditTrails { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
