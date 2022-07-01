namespace InvestTrackerWebApi.HttpApi.Controllers.Identity;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.HttpApi.Auth;
using InvestTrackerWebApi.HttpApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

public class UsersController : VersionNeutralApiController
{
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("all")]
    [MustHavePermission(CRUDAction.View, Resource.Users)]
    [OpenApiOperation("Get all users.", "")]
    public Task<List<UserDto>> GetAll([FromQuery] GetUsersQuery query, CancellationToken cancellationToken) =>
        this.Mediator.Send(query, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("{id:guid}")]
    [MustHavePermission(CRUDAction.View, Resource.Users)]
    [OpenApiOperation("Get user by id.", "")]
    public Task<UserDetailsDto> GetById(Guid id, CancellationToken cancellationToken) =>
         this.Mediator.Send(new GetUserQuery() { Id = id }, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet]
    [MustHavePermission(CRUDAction.View, Resource.Users)]
    [OpenApiOperation("Get users with pagination.", "")]
    public Task<PaginatedList<UserDto>> GetWithPagination([FromQuery] GetUsersWithPaginationQuery query, CancellationToken cancellationToken) =>
         this.Mediator.Send(query, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost]
    [AllowAnonymous]
    [OpenApiOperation("Create a new user.", "")]
    public Task<string> Create(CreateUserCommand request) =>
        this.Mediator.Send(request);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPut]
    [MustHavePermission(CRUDAction.Update, Resource.Users)]
    [OpenApiOperation("Update user", "")]
    public async Task<ActionResult> Update(UpdateUserCommand command)
    {
        await this.Mediator.Send(command);
        return this.Ok();
    }

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpDelete("{id:guid}")]
    [MustHavePermission(CRUDAction.Delete, Resource.Users)]
    [OpenApiOperation("Delete a user.", "")]
    public Task<string> Delete(Guid id) =>
        this.Mediator.Send(new DeleteUserCommand() { Id = id });

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("{id:guid}/roles")]
    [MustHavePermission(UsersOnlyAction.ViewRoles, Resource.Users)]
    [OpenApiOperation("Get user roles.", "")]
    public Task<UserWithRolesDto> GetByIdWithRoles(Guid id, CancellationToken cancellationToken) =>
        this.Mediator.Send(new GetUserWithRolesQuery() { Id = id }, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost("{id:guid}/roles")]
    [MustHavePermission(UsersOnlyAction.UpdateRoles, Resource.Users)]
    [OpenApiOperation("Update user roles.", "")]
    public async Task<ActionResult<string>> UpdateRoles(
        Guid id,
        AssignUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        if (id != request.UserId)
        {
            return this.BadRequest(new ErrorDto()
            {
                ExceptionMessage = "Request Id and Id in AssignUserRolesCommand are not same",
                ErrorId = Guid.Empty,
                SupportMessage = "Please contact support team for further analysis."
            });
        }

        return this.Ok(await this.Mediator.Send(request, cancellationToken));
    }

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost("toggle-status")]
    [MustHavePermission(CRUDAction.Update, Resource.Users)]
    [OpenApiOperation("Toggle user active status.", "")]
    public async Task<ActionResult> ToggleStatus(
        ToggleUserStatusCommand request,
        CancellationToken cancellationToken)
    {
        await this.Mediator.Send(request, cancellationToken);
        return this.Ok();
    }

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [OpenApiOperation("Confirm email address for user.", "")]
    public Task<string> ConfirmEmail(
        [FromQuery] Guid userId,
        [FromQuery] string code,
        CancellationToken cancellationToken) =>
        this.Mediator.Send(new ConfirmEmailCommand() { UserId = userId, Code = code }, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("confirm-phone-number")]
    [AllowAnonymous]
    [OpenApiOperation("Confirm phone number for user.", "")]
    public Task<string> ConfirmPhoneNumber([FromQuery] Guid userId, [FromQuery] string code) =>
        this.Mediator.Send(new ConfirmPhoneNumberCommand() { UserId = userId, Code = code });

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [OpenApiOperation("Request a pasword reset email for user.", "")]
    public Task<string> ForgotPassword(ForgotPasswordCommand request) =>
        this.Mediator.Send(request);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [OpenApiOperation("Reset user password.", "")]
    public Task<string> ResetPassword(ResetPasswordCommand request) =>
        this.Mediator.Send(request);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("export")]
    [AllowAnonymous]
    [OpenApiOperation("Export users.", "")]
    public async Task<FileResult> ExportAsync([FromQuery] ExportUsersRequest request)
    {
        var result = await this.Mediator.Send(request);
        return this.File(result, "application/octet-stream", "Users.xlsx");
    }
}
