namespace InvestTrackerWebApi.Application.Accounts;

using AutoMapper;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetCurrentUserAccountQuery : IRequest<CurrentUserAccountDetailsDto>
{
    public Guid Id { get; set; } = default!;
}

public class GetUserAccountQueryHandler : IRequestHandler<GetCurrentUserAccountQuery, CurrentUserAccountDetailsDto>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;
    private readonly ICurrentUser currentUser;

    public GetUserAccountQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ICurrentUser currentUser)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
        this.currentUser = currentUser;
    }

    public async Task<CurrentUserAccountDetailsDto> Handle(GetCurrentUserAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await this.applicationDbContext.Accounts.Where(account => account.Id == request.Id && account.UserId == this.currentUser.GetUserId())
            .FirstOrDefaultAsync(cancellationToken);

        if (account == null)
        {
            throw new NotFoundException(request.Id + " account not found for user " + this.currentUser.GetUserId());
        }

        return this.mapper.Map<CurrentUserAccountDetailsDto>(account);
    }
}
