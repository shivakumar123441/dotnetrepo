namespace InvestTrackerWebApi.Auditing.Configurations;

using InvestTrackerWebApi.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TrailConfiguration : IEntityTypeConfiguration<Trail>
{
    public void Configure(EntityTypeBuilder<Trail> builder)
    {
        _ = builder.Ignore(e => e.DomainEvents);
        _ = builder.ToTable("Trails", "audit");
    }
}
