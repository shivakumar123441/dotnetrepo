namespace InvestTrackerWebApi.Domain.Account;
using InvestTrackerWebApi.Domain.Common;

public class Account : AuditableEntity<Guid>
{
    public Account(Guid id, string referenceCode, string description, AccountType accountType, AccountStatus accountStatus, Guid userId, Guid currentUserId)
        : base(id, currentUserId, DateTime.UtcNow, currentUserId, DateTime.UtcNow)
    {
        this.ReferenceCode = referenceCode;
        this.Description = description;
        this.AccountType = accountType;
        this.AccountStatus = accountStatus;
        this.UserId = userId;
    }

    // used by ef core. should not be used for development.
    internal Account()
    {
    }

    public Account Update(string description, Guid currentUserId)
    {
        this.Description = description;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
        return this;
    }

    public string ReferenceCode { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public AccountType AccountType { get; private set; }

    public AccountStatus AccountStatus { get; private set; }

    public decimal Balance { get; private set; }

    public Guid UserId { get; private set; }

    public ICollection<Transaction.Transaction>? Transfers { get; private set; }
}
