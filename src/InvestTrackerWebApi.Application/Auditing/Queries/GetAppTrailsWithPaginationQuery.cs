namespace InvestTrackerWebApi.Application.Auditing;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Application.Pagination;
using MediatR;

public class GetAppTrailsWithPaginationQuery : PaginationQueryBase, IRequest<PaginatedList<AuditDto>>
{
    [ToLowerContainsComparison]
    public string? TableName { get; set; }

    [ToLowerContainsComparison]
    public string? Type { get; set; }
}

public class GetAppTrailsWithPaginationQueryHandler : IRequestHandler<GetAppTrailsWithPaginationQuery, PaginatedList<AuditDto>>
{
    private readonly IAuditDbContext auditDbContext;
    private readonly IMapper mapper;

    public GetAppTrailsWithPaginationQueryHandler(IAuditDbContext auditDbContext, IMapper mapper)
    {
        this.auditDbContext = auditDbContext;
        this.mapper = mapper;
    }

    public async Task<PaginatedList<AuditDto>> Handle(GetAppTrailsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var trails = this.auditDbContext.AuditTrails
            .ApplyFilter(request);
        return await trails.ProjectTo<AuditDto>(this.mapper.ConfigurationProvider)
           .PaginatedListAsync(request.CurrentPage, request.PageSize);
    }
}
