namespace InvestTrackerWebApi.Application.Identity.Roles;

using AutoMapper;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class GetRoleDetailsQuery : IRequest<RoleDetailsDto>
{
    public Guid Id { get; set; }
}

public class GetRoleQueryHandler : IRequestHandler<GetRoleDetailsQuery, RoleDetailsDto>
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IMapper mapper;

    public GetRoleQueryHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
    {
        this.roleManager = roleManager;
        this.mapper = mapper;
    }

    public async Task<RoleDetailsDto> Handle(GetRoleDetailsQuery request, CancellationToken cancellationToken)
    {
        var role = await this.roleManager.Roles.SingleOrDefaultAsync(x => x.Id == request.Id.ToString(), cancellationToken);

        if (role == null)
        {
            throw new NotFoundException("Role Not Found");
        }

        return this.mapper.Map<RoleDetailsDto>(role);
    }
}
