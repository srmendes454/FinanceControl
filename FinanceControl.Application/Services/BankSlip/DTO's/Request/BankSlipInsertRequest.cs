using System;

namespace FinanceControl.Application.Services.BankSlip.DTO_s.Request
{
    public class BankSlipInsertRequest
    {
        public Guid WalletId { get; set; }
        public string Name { get; set; }
        public int ExpirationDay { get; set; }
    }
}
