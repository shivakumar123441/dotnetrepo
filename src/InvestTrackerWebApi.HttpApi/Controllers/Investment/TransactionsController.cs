namespace InvestTrackerWebApi.HttpApi.Controllers;
using InvestTrackerWebApi.Application.Transactions;
using InvestTrackerWebApi.Application.Pagination;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.HttpApi.Auth;
using InvestTrackerWebApi.HttpApi.Models;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Microsoft.AspNetCore.Authorization;
using InvestTrackerWebApi.Application.FileStorage;

public class TransactionsController : VersionedApiController
{
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("all")]
    [MustHavePermission(CRUDAction.View, Resource.Transactions)]
    [OpenApiOperation("Get all transactions.", "")]
    public Task<List<TransactionDto>> GetAll([FromQuery] GetTransactionsQuery query, CancellationToken cancellationToken) =>
        this.Mediator.Send(query, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet("{id:guid}")]
    [MustHavePermission(CRUDAction.View, Resource.Transactions)]
    [OpenApiOperation("Get transaction by id.", "")]
    public Task<TransactionDetailsDto> GetById(Guid id) => this.Mediator.Send(new GetTransactionQuery(id));

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpGet]
    [MustHavePermission(CRUDAction.View, Resource.Transactions)]
    [OpenApiOperation("Get transactions with pagination.", "")]
    public Task<PaginatedList<TransactionDto>> GetWithPagination([FromQuery] GetTransactionsWithPaginationQuery query, CancellationToken cancellationToken) =>
         this.Mediator.Send(query, cancellationToken);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost]
    [MustHavePermission(CRUDAction.Create, Resource.Transactions)]
    [OpenApiOperation("Create a Transaction.", "")]
    public async Task<Guid> Create([FromForm] CreateTransactionRequestModel request)
    {
        var attachments = request.Attachments?.Select(x =>
        {
            var stream = new MemoryStream();
            x.CopyTo(stream);
            return new Attachment(x.FileName, x.ContentType, stream);
        }).ToList();

        var command = new CreateTransactionCommand()
        {
            FromAccountId = request.FromAccountId ?? new Guid(),
            ToAccountId = request.ToAccountId ?? new Guid(),
            TransactionType = request.TransactionType,
            Amount = request.Amount,
            UserComments = request.UserComments,
            TransactionComments = request.TransactionComments,
            MadeOn = request.MadeOn ?? new DateTime(),
            Attachments = attachments
        };
        return await this.Mediator.Send(command);
    }

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPut]
    [MustHavePermission(CRUDAction.Update, Resource.Transactions)]
    [OpenApiOperation("Update a transaction.", "")]
    public async Task<ActionResult> Update(UpdateTransactionCommand command) =>
        this.Ok(await this.Mediator.Send(command));

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [MustHavePermission(CRUDAction.Delete, Resource.Transactions)]
    [HttpDelete("{id}")]
    [OpenApiOperation("Delete a transaction.", "")]
    public async Task<Guid> Delete(Guid id) =>
        await this.Mediator.Send(new DeleteTransactionCommand(id));
}
