namespace InvestTrackerWebApi.Application.Accounts;

using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Domain.Account;

public class CurrentUserAccountDto : IMapFrom<Account>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; } = default!;
    public Guid UserId { get; set; }
}
