using System;

namespace FinanceControl.Application.Services.BankSlip.DTO_s.Response
{
    public class BankSlipResponse
    {
        public Guid BankSlipId { get; set; }
        public string Name { get; set; }
        public int ExpirationDay { get; set; }
    }
}
