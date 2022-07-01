namespace InvestTrackerWebApi.Application.Identity.Roles;

using AutoMapper;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;

public class RoleDetailsDto : IMapFrom<ApplicationRole>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ApplicationRole, RoleDetailsDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => Guid.Parse(s.Id)));
        profile.CreateMap<RoleDetailsDto, ApplicationRole>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()));
    }
}
