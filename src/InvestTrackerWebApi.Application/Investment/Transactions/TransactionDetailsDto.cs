namespace InvestTrackerWebApi.Application.Transactions;

using AutoMapper;
using InvestTrackerWebApi.Application.Mappings;
using InvestTrackerWebApi.Domain.FileStorage;
using InvestTrackerWebApi.Domain.Transaction;

public class TransactionDetailsDto : IMapFrom<Transaction>
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string? ReferenceCode { get; private set; }
    public string? TransactionType { get; set; }
    public DateTime MadeOn { get; set; }
    public decimal Amount { get; set; }
    public string? UserComments { get; private set; }
    public string? TransactionComments { get; private set; }
    public TransactionStatus TransactionStatus { get; private set; }
    public List<AttachmentStorageInfo>? Attachments { get; private set; }

    public void Mapping(Profile profile) => profile.CreateMap<Transaction, TransactionDetailsDto>()
        .ForMember(d => d.TransactionType, opt => opt.MapFrom(s => Enum.GetName(typeof(TransactionType), s.TransactionType)))
        .ForMember(d => d.TransactionStatus, opt => opt.MapFrom(s => Enum.GetName(typeof(TransactionStatus), s.TransactionStatus)));
}
