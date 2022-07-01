namespace InvestTrackerWebApi.Application.Identity.Users;

using AutoMapper;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;

public class CurrentUserDetailsDto : IMapFrom<ApplicationUser>
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ImageUrl { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ApplicationUser, CurrentUserDetailsDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => Guid.Parse(s.Id)));
        profile.CreateMap<CurrentUserDetailsDto, ApplicationUser>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()));
    }
}
