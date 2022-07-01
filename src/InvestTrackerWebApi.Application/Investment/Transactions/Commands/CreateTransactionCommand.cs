namespace InvestTrackerWebApi.Application.Transactions;

using InvestTrackerWebApi.Application.FileStorage;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Persistence;
using InvestTrackerWebApi.Domain.FileStorage;
using InvestTrackerWebApi.Domain.Transaction;
using MediatR;

public class CreateTransactionCommand : IRequest<Guid>
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string? UserComments { get; set; }
    public string? TransactionComments { get; set; }
    public DateTime MadeOn { get; set; }
    public List<Attachment>? Attachments { get; set; }
}

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Guid>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly ICurrentUser currentUser;
    private readonly IFileStorageService fileStorageService;

    public CreateTransactionCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUser currentUser, IFileStorageService fileStorageService)
    {
        this.applicationDbContext = applicationDbContext;
        this.currentUser = currentUser;
        this.fileStorageService = fileStorageService;
    }

    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var currentCount = this.applicationDbContext.Transactions.Count();
        var referenceCode = "TR" + string.Format("{0:0000000000}", currentCount + 1);
        Guid returnTransactionId = Guid.Empty;
        var attachmentUploadToPath = $"{this.currentUser.GetUserId()}/";

        switch (request.TransactionType)
        {
            case TransactionType.Deposit:
            {
                var transactionInToAccount = new Transaction(Guid.NewGuid(), request.ToAccountId, referenceCode, Domain.Transaction.TransactionType.Deposit, request.Amount, request.UserComments, request.TransactionComments, request.MadeOn, this.currentUser.GetUserId());
                _ = await this.applicationDbContext.Transactions.AddAsync(transactionInToAccount, cancellationToken);

                attachmentUploadToPath += $"{request.ToAccountId}/{transactionInToAccount.Id}/";
                var uploadedStorageInfos = await this.UploadAttachmentsAsync(request.Attachments, attachmentUploadToPath);
                _ = transactionInToAccount.LinkAttachments(uploadedStorageInfos);

                returnTransactionId = transactionInToAccount.Id;
                break;
            }

            case TransactionType.Withdraw:
            {
                var transactionInFromAccount = new Transaction(Guid.NewGuid(), request.FromAccountId, referenceCode, Domain.Transaction.TransactionType.Withdraw, request.Amount, request.UserComments, request.TransactionComments, request.MadeOn, this.currentUser.GetUserId());

                // TODO check for account balance and then do the withdrawl
                _ = await this.applicationDbContext.Transactions.AddAsync(transactionInFromAccount, cancellationToken);
                attachmentUploadToPath += $"{request.FromAccountId}/{transactionInFromAccount.Id}/";
                var uploadedStorageInfos = await this.UploadAttachmentsAsync(request.Attachments, attachmentUploadToPath);
                _ = transactionInFromAccount.LinkAttachments(uploadedStorageInfos);

                returnTransactionId = transactionInFromAccount.Id;
                break;
            }

            case TransactionType.Internal:
            {
                var transactionInFromAccount = new Transaction(Guid.NewGuid(), request.FromAccountId, referenceCode + "_Debit", Domain.Transaction.TransactionType.InternalDebit, request.Amount, request.UserComments, request.TransactionComments, request.MadeOn, this.currentUser.GetUserId());
                var transactionInToAccount = new Transaction(Guid.NewGuid(), request.ToAccountId, referenceCode + "_Credit", Domain.Transaction.TransactionType.InternalCredit, request.Amount, null, request.TransactionComments, request.MadeOn, this.currentUser.GetUserId());

                // TODO check for account balance and then do the withdrawl
                _ = await this.applicationDbContext.Transactions.AddAsync(transactionInFromAccount, cancellationToken);
                _ = await this.applicationDbContext.Transactions.AddAsync(transactionInToAccount, cancellationToken);
                returnTransactionId = transactionInFromAccount.Id;
                break;
            }

            default:
                break;
        }

        _ = await this.applicationDbContext.SaveChangesAsync(cancellationToken);
        return returnTransactionId;
    }

    private async Task<List<AttachmentStorageInfo>> UploadAttachmentsAsync(List<Attachment>? attachments, string attachmentUploadToPath)
    {
        List<AttachmentStorageInfo> attachmentStorageInfos = new List<AttachmentStorageInfo>();
        if (attachments is not null)
        {
            foreach (var attachment in attachments)
            {
                var storageInfo = await this.fileStorageService.UploadAsync<Attachment>(attachment, attachmentUploadToPath);
                if (storageInfo is not null)
                {
                    attachmentStorageInfos.Add(storageInfo);
                }
            }
        }

        return attachmentStorageInfos;
    }
}
