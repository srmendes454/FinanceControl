using System;

namespace FinanceControl.Application.Services.Transactions.DTO_s.Request
{
    public class TransactionsInsertRequest
    {
        public Guid AssignedId { get; set; }
        public string Name { get; set; }
        public bool Installment { get; set; }
        public double? Value { get; set; }
        public string Type { get; set; }
        public string CashFlow { get; set; }
        public string ExpenseType { get; set; }
        public DateTime DatePurchase { get; set; }
        public TransactionsRepetitionInsertRequest Repetition { get; set; }
    }

    public class TransactionsRepetitionInsertRequest
    {
        public int QuantityInstallment { get; set; }
        public int CurrentInstallment { get; set; } = 1;
        public double ValueInstallment { get; set; }
    }

    public class TransactionsEvaluateAssignedRequest
    {
        public Guid TransactionId { get; set; }
        public Guid WalletId { get; set; }
        public Guid CardId { get; set; }
        public bool Approved { get; set; }
    }

    public class InvestedAmountRequest
    {
        public Guid AccountBankId { get; set; }
        public double ValueRedeemed { get; set; }
    }

    public class RedeemInvestedAmountRequest
    {
        public Guid AccountBankId { get; set; }
        public bool FullAmount { get; set; }
        public double ValueRedeemed { get; set; }
    }
}
