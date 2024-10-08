using System;
using System.Collections.Generic;

namespace FinanceControl.Application.Services.CardBill.DTO_s.Request
{
    public class CardBillResponse
    {
        public Guid CardId { get; set; }
        public Guid CardBillId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public double TotalValue { get; set; }
        public string Status { get; set; }
        public List<CardBillTransactionsResponse> Transactions { get; set; }
    }

    public class CardBillTransactionsResponse
    {
        public Guid TransactionId { get; set; }
        public DateTime DatePurchase { get; set; }
        public double Value { get; set; }
        public string Name { get; set; }
        public int NumberInstallments { get; set; }
        public int CurrentInstallment { get; set; }
    }
}
