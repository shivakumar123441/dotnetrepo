namespace InvestTrackerWebApi.Persistence;
using System.Data;
using System.Reflection;
using InvestTrackerWebApi.Application.Auditing;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Application.Persistence;
using InvestTrackerWebApi.Application.Serialization;
using InvestTrackerWebApi.Domain.Account;
using InvestTrackerWebApi.Domain.Common;
using InvestTrackerWebApi.Domain.Transaction;
using InvestTrackerWebApi.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IEventService? eventService;
    private readonly ILoggerFactory? loggerFactory;
    private readonly ISerializerService? serializer;
    private readonly IAuditDbContext? auditDbContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IEventService? eventService,
        ILoggerFactory? loggerFactory,
        ISerializerService? serializer,
        IAuditDbContext? auditDbContext)
    : base(options)
    {
        this.eventService = eventService;
        this.loggerFactory = loggerFactory;
        this.serializer = serializer;
        this.auditDbContext = auditDbContext;
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILoggerFactory loggerFactory)
        : base(options) => this.loggerFactory = loggerFactory;

    public DbSet<Account> Accounts => this.Set<Account>();

    public DbSet<Transaction> Transactions => this.Set<Transaction>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        List<AuditTrail>? auditTrails = null;
        if (this.auditDbContext != null && this.serializer != null)
        {
            auditTrails = this.HandleAuditingBeforeSaveChanges(this.serializer, this.auditDbContext);
        }

        int results = await base.SaveChangesAsync(cancellationToken);

        if (this.auditDbContext != null && auditTrails != null)
        {
            await this.HandleAuditingAfterSaveChangesAsync(auditTrails, this.auditDbContext);
        }

        if (this.eventService == null)
        {
            return results;
        }
        else
        {
            await this.SendDomainEventsAsync(this.eventService);
            return results;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        _ = modelBuilder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);
        base.OnModelCreating(modelBuilder);

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder.UseLoggerFactory(this.loggerFactory);
        _ = optionsBuilder.EnableSensitiveDataLogging();
    }

    private List<AuditTrail> HandleAuditingBeforeSaveChanges(ISerializerService serializer, IAuditDbContext auditDbContext)
    {
        var trailEntries = new List<AuditTrail>();
        foreach (var entry in this.ChangeTracker.Entries<IAuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
            .ToList())
        {
            var trailEntry = new AuditTrail(entry, serializer)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = entry.Entity.LastModifiedBy
            };
            trailEntries.Add(trailEntry);
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    trailEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    trailEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        trailEntry.TrailType = TrailType.Create;
                        trailEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        trailEntry.TrailType = TrailType.Delete;
                        trailEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified && entry.Entity is ISoftDelete && property.OriginalValue == null && property.CurrentValue != null)
                        {
                            trailEntry.ChangedColumns.Add(propertyName);
                            trailEntry.TrailType = TrailType.Delete;
                            trailEntry.OldValues[propertyName] = property.OriginalValue;
                            trailEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        else if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                        {
                            trailEntry.ChangedColumns.Add(propertyName);
                            trailEntry.TrailType = TrailType.Update;
                            trailEntry.OldValues[propertyName] = property.OriginalValue;
                            trailEntry.NewValues[propertyName] = property.CurrentValue;
                        }

                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        break;
                }
            }
        }

        foreach (var auditEntry in trailEntries.Where(e => !e.HasTemporaryProperties))
        {
            auditDbContext.AuditTrails.Add(auditEntry.ToAuditTrail());
        }

        return trailEntries.Where(e => e.HasTemporaryProperties).ToList();
    }

    private Task HandleAuditingAfterSaveChangesAsync(List<AuditTrail> trailEntries, IAuditDbContext auditDbContext, CancellationToken cancellationToken = new())
    {
        foreach (var entry in trailEntries)
        {
            foreach (var prop in entry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    entry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                }
                else
                {
                    entry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }

            auditDbContext.AuditTrails.Add(entry.ToAuditTrail());
        }

        return auditDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SendDomainEventsAsync(IEventService eventService)
    {
        var entitiesWithEvents = this.ChangeTracker.Entries<IHasDomainEvent>()
                                                .Select(e => e.Entity)
                                                .Where(e => e.DomainEvents.Count > 0)
                                                .ToArray();

        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.DomainEvents.Clear();
            foreach (var @event in events)
            {
                await eventService.PublishAsync(@event);
            }
        }
    }
}
