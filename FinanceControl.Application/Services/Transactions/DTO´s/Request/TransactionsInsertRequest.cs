using System;

namespace FinanceControl.Application.Services.Transactions.DTO_s.Request
{
    public class TransactionsInsertRequest
    {
        public Guid WalletId { get; set; }
        public Guid Id { get; set; }
        public string AssignedEmail { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string CashFlow { get; set; }
        public DateTime DatePurchase { get; set; }
        public TransactionsRepetitionInsertRequest Repetition { get; set; }
    }

    public class TransactionsRepetitionInsertRequest
    {
        public int QuantityInstallment { get; set; }
        public int CurrentInstallment { get; set; } = 1;
        public double ValueInstallment { get; set; }
    }
}
