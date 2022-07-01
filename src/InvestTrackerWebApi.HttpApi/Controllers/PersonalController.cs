namespace InvestTrackerWebApi.HttpApi.Controllers;

using InvestTrackerWebApi.Application.Accounts;
using InvestTrackerWebApi.Application.Auditing;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Application.Transactions;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.HttpApi.Auth;
using InvestTrackerWebApi.HttpApi.Extensions;
using InvestTrackerWebApi.HttpApi.Models;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

public class PersonalController : VersionNeutralApiController
{
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("profile")]
    [MustHavePermission(PersonalOnlyAction.ViewUserDetails, Resource.Personal)]
    [OpenApiOperation("Get profile details of currently logged in user.", "")]
    public async Task<CurrentUserDetailsDto> GetProfile(CancellationToken cancellationToken) =>
        await this.Mediator.Send(new GetCurrentUserDetailsQuery(), cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPut("profile")]
    [MustHavePermission(PersonalOnlyAction.UpdateUserDetails, Resource.Personal)]
    [OpenApiOperation("Update profile details of currently logged in user.", "")]
    public Task UpdateProfile(UpdateCurrentUserCommand command) =>
        this.Mediator.Send(command);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPut("change-password")]
    [MustHavePermission(PersonalOnlyAction.UpdatePassword, Resource.Personal)]
    [OpenApiOperation("Change password of currently logged in user.", "")]
    public Task ChangePassword(ChangeCurrentUserPasswordCommand command) =>
        this.Mediator.Send(command);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("permissions")]
    [MustHavePermission(PersonalOnlyAction.ViewPermissions, Resource.Personal)]
    [OpenApiOperation("Get permissions of currently logged in user.", "")]
    public async Task<List<string>> GetPermissions(CancellationToken cancellationToken) =>
        await this.Mediator.Send(new GetCurrentUserPermissionsQuery(), cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("accounts")]
    [MustHavePermission(PersonalOnlyAction.ViewAccounts, Resource.Personal)]
    [OpenApiOperation("Get accounts of currently logged in user.", "")]
    public Task<PaginatedList<CurrentUserAccountDto>> GetAccountsWithPagination([FromQuery] GetCurrentUserAccountsWithPaginationQuery query, CancellationToken cancellationToken) =>
        this.Mediator.Send(query, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("accounts/{id:guid}")]
    [MustHavePermission(PersonalOnlyAction.ViewAccountDetails, Resource.Personal)]
    [OpenApiOperation("Get user account by id.", "")]
    public Task<CurrentUserAccountDetailsDto> GetAccountById(Guid id, CancellationToken cancellationToken) =>
         this.Mediator.Send(new GetCurrentUserAccountQuery() { Id = id }, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("transactions")]
    [MustHavePermission(PersonalOnlyAction.ViewTransactions, Resource.Personal)]
    [OpenApiOperation("Get transactions of currently logged in user.", "")]
    public Task<PaginatedList<CurrentUserTransactionDto>> GetTransactionsWithPagination([FromQuery] GetCurrentUserTransactionsWithPaginationQuery query, CancellationToken cancellationToken) =>
        this.Mediator.Send(query, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("transactions/{id:guid}")]
    [MustHavePermission(PersonalOnlyAction.ViewTransactionDetails, Resource.Personal)]
    [OpenApiOperation("Get user account by id.", "")]
    public Task<CurrentUserTransactionDetailsDto> GetTransactionById(Guid id, CancellationToken cancellationToken) =>
         this.Mediator.Send(new GetCurrentUserTransactionQuery() { Id = id, UserId = this.User.GetUserId() }, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("logs")]
    [MustHavePermission(PersonalOnlyAction.ViewLogs, Resource.Personal)]
    [OpenApiOperation("Get audit logs of currently logged in user.", "")]
    public Task<PaginatedList<AuditDto>> GetLogsWithPagination([FromQuery] GetUserTrailsWithPaginationQuery query, CancellationToken cancellationToken) =>
        this.Mediator.Send(query, cancellationToken);
}
