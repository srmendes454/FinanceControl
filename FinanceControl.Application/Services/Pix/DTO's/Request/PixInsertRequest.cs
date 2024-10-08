using System;

namespace FinanceControl.Application.Services.Pix.DTO_s.Request
{
    public class PixInsertRequest
    {
        public Guid WalletId { get; set; }
        public string Name { get; set; }
        public string LinkedAccount { get; set; }
        public int ExpirationDay { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
    }
}
