namespace InvestTrackerWebApi.Application.Auditing;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Application.Pagination;
using MediatR;

public class GetUserTrailsWithPaginationQuery : PaginationQueryBase, IRequest<PaginatedList<AuditDto>>
{
    [ToLowerContainsComparison]
    public string? TableName { get; set; }

    [ToLowerContainsComparison]
    public string? Type { get; set; }
}

public class GetUserTrailsWithPaginationQueryHandler : IRequestHandler<GetUserTrailsWithPaginationQuery, PaginatedList<AuditDto>>
{
    private readonly IAuditDbContext auditDbContext;
    private readonly ICurrentUser currentUser;
    private readonly IMapper mapper;

    public GetUserTrailsWithPaginationQueryHandler(IAuditDbContext auditDbContext, ICurrentUser currentUser, IMapper mapper)
    {
        this.auditDbContext = auditDbContext;
        this.currentUser = currentUser;
        this.mapper = mapper;
    }

    public async Task<PaginatedList<AuditDto>> Handle(GetUserTrailsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var trails = this.auditDbContext.AuditTrails
            .Where(a => a.UserId == this.currentUser.GetUserId())
            .ApplyFilter(request);
        return await trails.ProjectTo<AuditDto>(this.mapper.ConfigurationProvider)
           .PaginatedListAsync(request.CurrentPage, request.PageSize);
    }
}
