namespace InvestTrackerWebApi.Persistence.Configurations;

using InvestTrackerWebApi.Domain.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        _ = builder.Ignore(e => e.DomainEvents);
        _ = builder.HasIndex(e => e.ReferenceCode).IsUnique();
    }
}
