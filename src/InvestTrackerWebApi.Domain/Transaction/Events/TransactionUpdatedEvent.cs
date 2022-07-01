namespace InvestTrackerWebApi.Domain.Transaction.Events;
using InvestTrackerWebApi.Domain.Common;

public class TransactionUpdatedEvent : DomainEvent
{
    public TransactionUpdatedEvent(Transaction transaction) => this.Transaction = transaction;

    public Transaction Transaction { get; }
}
