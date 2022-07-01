namespace InvestTrackerWebApi.Application.Identity.Users;

public class UserWithRolesDto
{
    public Guid Id { get; set; } = default!;

    public List<UserRoleDto>? UserRolesDto { get; set; }
}
