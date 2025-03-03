using System;

namespace FinanceControl.Application.Services.Division.DTO_s.Request
{
    public class DivisionInsertRequest
    {
        public Guid WalletId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double Percent { get; set; }
    }
}
