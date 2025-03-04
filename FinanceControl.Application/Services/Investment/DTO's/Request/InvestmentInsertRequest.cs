using System;

namespace FinanceControl.Application.Services.Investment.DTO_s.Request
{
    public class InvestmentInsertRequest
    {
        public Guid WalletId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double MonthlyProfitability { get; set; }
        public string Type { get; set; }
    }
}
