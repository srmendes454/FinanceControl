using System;

namespace FinanceControl.Application.Services.Investment.DTO_s.Response
{
    public class InvestmentResponse
    {
        public Guid InvestmentId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double MonthlyProfitability { get; set; }
        public string Type { get; set; }
    }

    public class InvestmentAllResponse
    {
        public Guid InvestmentId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double AmountInvested { get; set; }
        public string Type { get; set; }
    }
}
