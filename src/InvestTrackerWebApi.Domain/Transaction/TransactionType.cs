namespace InvestTrackerWebApi.Domain.Transaction;

public enum TransactionType
{
    Deposit = 0,
    Withdraw = 1,
    InternalCredit = 2,
    InternalDebit = 3,
}
