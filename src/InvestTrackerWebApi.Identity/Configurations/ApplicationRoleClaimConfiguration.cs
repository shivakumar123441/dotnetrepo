namespace InvestTrackerWebApi.Identity.Configurations;

using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRoleClaim;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {
        _ = builder.Ignore(e => e.DomainEvents);
        _ = builder.ToTable(TableNames.ApplicationRoleClaims, "identity");
    }
}
