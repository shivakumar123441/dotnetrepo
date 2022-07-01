namespace InvestTrackerWebApi.Application.Identity.Users;

using AutoMapper;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class GetUserQuery : IRequest<UserDetailsDto>
{
    public Guid Id { get; set; } = default!;
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDetailsDto>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IMapper mapper;

    public GetUserQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        this.userManager = userManager;
        this.mapper = mapper;
    }

    public async Task<UserDetailsDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.Users.SingleOrDefaultAsync(x => x.Id == request.Id.ToString(), cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User Not Found");
        }

        return this.mapper.Map<UserDetailsDto>(user);
    }
}
