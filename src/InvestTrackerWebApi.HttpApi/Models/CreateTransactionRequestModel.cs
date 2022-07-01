namespace InvestTrackerWebApi.HttpApi.Models;
using System;
using InvestTrackerWebApi.Application.Transactions;
using Microsoft.AspNetCore.Http;

public class CreateTransactionRequestModel
{
    public Guid? FromAccountId { get; set; }
    public Guid? ToAccountId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string? UserComments { get; set; }
    public string? TransactionComments { get; set; }
    public DateTime? MadeOn { get; set; }
    public List<IFormFile>? Attachments { get; set; }
}
