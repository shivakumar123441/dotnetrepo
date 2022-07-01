namespace InvestTrackerWebApi.Domain.Transaction.Events;
using InvestTrackerWebApi.Domain.Common;

public class TransactionDeletedEvent : DomainEvent
{
    public TransactionDeletedEvent(Transaction transaction) => this.Transaction = transaction;

    public Transaction Transaction { get; }
}
