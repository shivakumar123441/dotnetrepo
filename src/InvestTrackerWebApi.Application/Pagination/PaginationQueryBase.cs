namespace InvestTrackerWebApi.Application.Pagination;

using AutoFilterer.Attributes;
using AutoFilterer.Types;

public class PaginationQueryBase : OrderableFilterBase
{
    [IgnoreFilter]
    public int CurrentPage { get; set; } = 1;

    [IgnoreFilter]
    public int PageSize { get; set; } = 10;
}
