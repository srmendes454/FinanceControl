using System;

namespace FinanceControl.Application.Services.Transactions.DTO_s.Request
{
    public class TransactionsUpdateRequest
    {
        public Guid AssignedId { get; set; }
        public string Name { get; set; }
        public bool Installment { get; set; }
        public bool UpdateAll { get; set; }
        public double? Value { get; set; }
        public string Type { get; set; }
        public string CashFlow { get; set; }
        public string ExpenseType { get; set; }
        public DateTime DatePurchase { get; set; }
        public TransactionsRepetitionInsertRequest Repetition { get; set; }
    }
}
