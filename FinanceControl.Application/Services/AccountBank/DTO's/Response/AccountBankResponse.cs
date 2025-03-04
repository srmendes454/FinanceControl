using System;

namespace FinanceControl.Application.Services.AccountBank.DTO_s.Response
{
    public class AccountBankResponse
    {
        public Guid AccountBankId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
    }

    public class AccountBankAllResponse
    {
        public Guid AccountBankId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public double AmountDisponible { get; set; }
    }
}
