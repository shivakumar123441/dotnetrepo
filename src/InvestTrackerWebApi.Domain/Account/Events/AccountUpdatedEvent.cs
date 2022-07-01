namespace InvestTrackerWebApi.Domain.Account.Events;
using InvestTrackerWebApi.Domain.Common;

public class AccountUpdatedEvent : DomainEvent
{
    public AccountUpdatedEvent(Account account) => this.Account = account;

    public Account Account { get; }
}
