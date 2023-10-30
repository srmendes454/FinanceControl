using System;

namespace FinanceControl.Application.Services.Transactions.DTO_s.Response
{
    public class TransactionsResponse
    {
        public Guid TransactionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string CashFlow { get; set; }
        public DateTime DatePurchase { get; set; }
        public TransactionsRepetitionResponse Repetition { get; set; }
        public TransactionsAssignedResponse Assigned { get; set; }
    }

    public class TransactionsRepetitionResponse
    {
        public Guid RepetitionId { get; set; }
        public int QuantityInstallment { get; set; }
        public int CurrentInstallment { get; set; } = 1;
        public double ValueInstallment { get; set; }
    }

    public class TransactionsPaymentDetailsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public class TransactionsAssignedResponse
    {
        public Guid AssignedId { get; set; }
        public string Name { get; set; }
    }

    public class TransactionsAssignedToMeResponse
    {
        public Guid TransactionId { get; set; }
        public string Marked { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string CashFlow { get; set; }
        public DateTime DatePurchase { get; set; }
        public int QuantityInstallment { get; set; }
        public int CurrentInstallment { get; set; } = 1;
        public double ValueInstallment { get; set; }
    }
}
