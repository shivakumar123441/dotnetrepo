namespace InvestTrackerWebApi.Application.Transactions;

using AutoMapper;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetTransactionQuery : IRequest<TransactionDetailsDto>
{
    public Guid Id { get; set; }

    public GetTransactionQuery(Guid id) => this.Id = id;
}

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionDetailsDto>
{
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IMapper mapper;

    public GetTransactionQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        this.applicationDbContext = applicationDbContext;
        this.mapper = mapper;
    }

    public async Task<TransactionDetailsDto> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await this.applicationDbContext.Transactions
            .Where(x => x.Id == request.Id)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync();

        if (transaction == null)
        {
            throw new NotFoundException(request.Id + " transaction not found");
        }

        return this.mapper.Map<TransactionDetailsDto>(transaction);
    }
}
