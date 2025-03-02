using System;

namespace FinanceControl.Application.Services.BankSlip.DTO_s.Request
{
    public class BankSlipUpdateRequest
    {
        public string Name { get; set; }
        public int ExpirationDay { get; set; }
    }
}
