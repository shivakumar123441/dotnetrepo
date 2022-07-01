namespace InvestTrackerWebApi.HttpApi.Controllers.Identity;
using InvestTrackerWebApi.Application.Identity.Roles;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.HttpApi.Auth;
using InvestTrackerWebApi.HttpApi.Models;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

public class RolesController : VersionNeutralApiController
{
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("all")]
    [MustHavePermission(CRUDAction.View, Resource.Roles)]
    [OpenApiOperation("Get all roles.", "")]
    public Task<List<RoleDto>> GetAll([FromQuery] GetRolesQuery query, CancellationToken cancellationToken) =>
        this.Mediator.Send(query, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("{id:guid}")]
    [MustHavePermission(CRUDAction.View, Resource.Roles)]
    [OpenApiOperation("Get role by id.", "")]
    public Task<RoleDetailsDto> GetById(Guid id, CancellationToken cancellationToken) =>
        this.Mediator.Send(new GetRoleDetailsQuery() { Id = id }, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet]
    [MustHavePermission(CRUDAction.View, Resource.Roles)]
    [OpenApiOperation("Get roles with pagination.", "")]
    public Task<PaginatedList<RoleDto>> GetWithPagination([FromQuery] GetRolesWithPaginationQuery query, CancellationToken cancellationToken) =>
         this.Mediator.Send(query, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost]
    [MustHavePermission(CRUDAction.Create, Resource.Roles)]
    [OpenApiOperation("Create a role.", "")]
    public Task<string> Create(CreateRoleCommand request) =>
        this.Mediator.Send(request);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPut]
    [MustHavePermission(CRUDAction.Update, Resource.Roles)]
    [OpenApiOperation("Update a role.", "")]
    public Task<string> Update(UpdateRoleCommand request) =>
        this.Mediator.Send(request);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpDelete("{id:guid}")]
    [MustHavePermission(CRUDAction.Delete, Resource.Roles)]
    [OpenApiOperation("Delete a role.", "")]
    public Task<string> Delete(Guid id) =>
        this.Mediator.Send(new DeleteRoleCommand() { Id = id });

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("{id:guid}/permissions")]
    [MustHavePermission(RolesOnlyAction.ViewRoleClaims, Resource.Roles)]
    [OpenApiOperation("Get role with its permissions.", "")]
    public Task<RoleWithPermissionsDto> GetByIdWithPermissions(Guid id, CancellationToken cancellationToken) =>
    this.Mediator.Send(new GetRoleWithPermissionsQuery() { Id = id }, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost("{id:guid}/permissions")]
    [MustHavePermission(RolesOnlyAction.UpdateRoleClaims, Resource.Roles)]
    [OpenApiOperation("Update role permissions.", "")]
    public async Task<ActionResult<string>> UpdatePermissions(
        Guid id,
        UpdateRolePermissionsCommand request,
        CancellationToken cancellationToken)
    {
        if (id != request.RoleId)
        {
            return this.BadRequest(new ErrorDto()
            {
                ExceptionMessage = "Request Id and Id in UpdateRolePermissionsRequest are not same",
                ErrorId = Guid.Empty,
                SupportMessage = "Please contact support team for further analysis."
            });
        }

        return this.Ok(await this.Mediator.Send(request, cancellationToken));
    }
}
