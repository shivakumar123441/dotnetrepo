namespace InvestTrackerWebApi.Domain.Account.Events;
using InvestTrackerWebApi.Domain.Common;

public class AccountCreatedEvent : DomainEvent
{
    public AccountCreatedEvent(Account account) => this.Account = account;

    public Account Account { get; }
}
