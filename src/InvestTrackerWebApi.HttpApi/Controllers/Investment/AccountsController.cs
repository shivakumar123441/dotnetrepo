namespace InvestTrackerWebApi.HttpApi.Controllers;
using InvestTrackerWebApi.Application.Accounts;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.HttpApi.Auth;
using InvestTrackerWebApi.HttpApi.Models;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

public class AccountsController : VersionedApiController
{
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("all")]
    [MustHavePermission(CRUDAction.View, Resource.Accounts)]
    [OpenApiOperation("Get all accounts.", "")]
    public Task<List<AccountDto>> GetAll([FromQuery] GetAccountsQuery query, CancellationToken cancellationToken) =>
        this.Mediator.Send(query, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("{id:guid}")]
    [MustHavePermission(CRUDAction.View, Resource.Accounts)]
    [OpenApiOperation("Get account by id.", "")]
    public Task<AccountDetailsDto> GetById(Guid id) => this.Mediator.Send(new GetAccountQuery(id));

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet]
    [MustHavePermission(CRUDAction.View, Resource.Accounts)]
    [OpenApiOperation("Get roles with pagination.", "")]
    public Task<PaginatedList<AccountDto>> GetWithPagination([FromQuery] GetAccountsWithPaginationQuery query, CancellationToken cancellationToken) =>
         this.Mediator.Send(query, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost]
    [MustHavePermission(CRUDAction.Create, Resource.Accounts)]
    [OpenApiOperation("Create an account.", "")]
    public async Task<Guid> Create(CreateFixedDepositAccountCommand command) =>
        await this.Mediator.Send(command);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPut]
    [MustHavePermission(CRUDAction.Update, Resource.Accounts)]
    [OpenApiOperation("Update an account.", "")]
    public async Task<ActionResult> Update(UpdateAccountCommand command) =>
        this.Ok(await this.Mediator.Send(command));

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [MustHavePermission(CRUDAction.Delete, Resource.Accounts)]
    [HttpDelete("{id}")]
    [OpenApiOperation("Delete an account.", "")]
    public async Task<Guid> Delete(Guid id) =>
        await this.Mediator.Send(new DeleteAccountCommand(id));
}
