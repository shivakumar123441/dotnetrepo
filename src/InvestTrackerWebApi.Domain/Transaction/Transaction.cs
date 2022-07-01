namespace InvestTrackerWebApi.Domain.Transaction;
using InvestTrackerWebApi.Domain.Common;
using InvestTrackerWebApi.Domain.FileStorage;

public class Transaction : AuditableEntity<Guid>
{
    public Guid AccountId { get; private set; }
    public string? ReferenceCode { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public decimal Amount { get; private set; }
    public string? UserComments { get; private set; }
    public string? TransactionComments { get; private set; }
    public DateTime MadeOn { get; private set; }
    public TransactionStatus TransactionStatus { get; private set; }
    public List<AttachmentStorageInfo> Attachments { get; private set; }

    public Transaction(Guid id, Guid accountId, string? referenceCode, TransactionType transactionType, decimal amount, string? userComments, string? transactionComments, DateTime madeOn, Guid currentUserId)
        : base(id, currentUserId, DateTime.UtcNow, currentUserId, DateTime.UtcNow)
    {
        this.AccountId = accountId;
        this.ReferenceCode = referenceCode;
        this.TransactionType = transactionType;
        this.Amount = amount;
        this.UserComments = userComments;
        this.TransactionComments = transactionComments;
        this.MadeOn = madeOn;
        this.Attachments = new List<AttachmentStorageInfo>();
    }

    // used by ef core. should not be used for development.
    internal Transaction()
    {
    }

    public Transaction UpdateUserComments(string? userComments)
    {
        this.UserComments = userComments;
        return this;
    }

    public Transaction UpdateTransactionComments(string? transactionComments)
    {
        this.TransactionComments = transactionComments;
        return this;
    }

    public Transaction UpdateMadeOnDate(DateTime madeOn)
    {
        this.MadeOn = madeOn;
        return this;
    }

    public Transaction LinkAttachments(List<AttachmentStorageInfo> attachmentsStorageInfo)
    {
        this.Attachments.AddRange(attachmentsStorageInfo);
        return this;
    }

    public Transaction DelinkAttachments(List<Guid> attachments)
    {
        _ = this.Attachments?.RemoveAll(x => attachments.Contains(x.Id));
        return this;
    }

}


