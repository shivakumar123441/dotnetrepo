namespace InvestTrackerWebApi.Persistence.Configurations;

using InvestTrackerWebApi.Domain.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        _ = builder.Ignore(e => e.DomainEvents);
        _ = builder.HasIndex(e => e.ReferenceCode).IsUnique();
    }
}
