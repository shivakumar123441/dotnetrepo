namespace InvestTrackerWebApi.Application.Identity.Users;

using System.Threading.Tasks;
using AutoMapper;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class GetCurrentUserDetailsQuery : IRequest<CurrentUserDetailsDto>
{
}

public class GetUserDetailsQueryHandler : IRequestHandler<GetCurrentUserDetailsQuery, CurrentUserDetailsDto>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IMapper mapper;
    private readonly ICurrentUser currentUser;

    public GetUserDetailsQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper, ICurrentUser currentUser)
    {
        this.userManager = userManager;
        this.mapper = mapper;
        this.currentUser = currentUser;
    }

    public async Task<CurrentUserDetailsDto> Handle(GetCurrentUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.Users.SingleOrDefaultAsync(x => x.Id == this.currentUser.GetUserId().ToString(), cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User Not Found");
        }

        return this.mapper.Map<CurrentUserDetailsDto>(user);
    }
}
