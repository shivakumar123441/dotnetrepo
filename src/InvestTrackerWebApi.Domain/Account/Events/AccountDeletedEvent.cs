namespace InvestTrackerWebApi.Domain.Account.Events;
using InvestTrackerWebApi.Domain.Common;

public class AccountDeletedEvent : DomainEvent
{
    public AccountDeletedEvent(Account account) => this.Account = account;

    public Account Account { get; }
}
