namespace InvestTrackerWebApi.Domain.Transaction.Events;
using InvestTrackerWebApi.Domain.Common;

public class TransactionCreatedEvent : DomainEvent
{
    public TransactionCreatedEvent(Transaction transaction) => this.Transaction = transaction;

    public Transaction Transaction { get; }
}
