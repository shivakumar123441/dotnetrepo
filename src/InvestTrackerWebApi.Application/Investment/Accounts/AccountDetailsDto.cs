namespace InvestTrackerWebApi.Application.Accounts;

using AutoMapper;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Domain.Account;

public class AccountDetailsDto : IMapFrom<Account>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; } = default!;
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public string? AccountType { get; set; }
    public string? AccountStatus { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Account, AccountDetailsDto>()
            .ForMember(d => d.AccountType, opt => opt.MapFrom(s => Enum.GetName(typeof(AccountType), s.AccountType)));

        profile.CreateMap<Account, AccountDetailsDto>()
            .ForMember(d => d.AccountStatus, opt => opt.MapFrom(s => Enum.GetName(typeof(AccountStatus), s.AccountStatus)));
    }
}
