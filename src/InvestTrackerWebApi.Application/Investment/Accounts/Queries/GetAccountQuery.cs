namespace InvestTrackerWebApi.Application.Accounts;

using AutoMapper;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;

public class GetAccountQuery : IRequest<AccountDetailsDto>
{
    public Guid Id { get; set; }

    public GetAccountQuery(Guid id) => this.Id = id;
}

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDetailsDto>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;

    public GetAccountQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
    }

    public async Task<AccountDetailsDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await this.applicationDbContext.Accounts
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (account == null)
        {
            throw new NotFoundException(request.Id + " account not found");
        }

        return this.mapper.Map<AccountDetailsDto>(account);
    }
}
