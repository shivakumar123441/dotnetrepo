namespace InvestTrackerWebApi.Identity.Configurations;

using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        _ = builder.Ignore(e => e.DomainEvents);
        _ = builder.ToTable(TableNames.ApplicationUsers, "identity");
    }
}
