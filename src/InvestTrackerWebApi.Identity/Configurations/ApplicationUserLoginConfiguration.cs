namespace InvestTrackerWebApi.Identity.Configurations;

using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApplicationUserLoginConfiguration : IEntityTypeConfiguration<ApplicationUserLogin>
{
    public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder) =>
        builder.ToTable(TableNames.ApplicationUserLogins, "identity");
}
