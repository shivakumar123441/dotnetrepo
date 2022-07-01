namespace InvestTrackerWebApi.Identity.Configurations;

using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApplicationUserTokenConfiguration : IEntityTypeConfiguration<ApplicationUserToken>
{
    public void Configure(EntityTypeBuilder<ApplicationUserToken> builder) =>
        builder.ToTable(TableNames.ApplicationUserTokens, "identity");
}
