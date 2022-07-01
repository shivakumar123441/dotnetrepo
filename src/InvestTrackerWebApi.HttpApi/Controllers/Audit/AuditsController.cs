namespace InvestTrackerWebApi.HttpApi.Controllers;
using InvestTrackerWebApi.Application.Auditing;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.HttpApi.Auth;
using InvestTrackerWebApi.HttpApi.Models;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

public class AuditsController : VersionedApiController
{
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet]
    [MustHavePermission(CRUDAction.View, Resource.Audits)]
    [OpenApiOperation("Get audits with pagination.", "")]
    public Task<PaginatedList<AuditDto>> GetWithPagination([FromQuery] GetAppTrailsWithPaginationQuery query, CancellationToken cancellationToken) =>
        this.Mediator.Send(query, cancellationToken);
}
