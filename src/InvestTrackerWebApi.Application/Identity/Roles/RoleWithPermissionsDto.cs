namespace InvestTrackerWebApi.Application.Identity.Roles;

using InvestTrackerWebApi.Application.Mappings;

public class RoleWithPermissionsDto : IMapFrom<RoleDetailsDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string>? Permissions { get; set; }
}
