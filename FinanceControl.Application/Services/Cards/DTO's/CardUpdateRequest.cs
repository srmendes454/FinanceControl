using System;

namespace FinanceControl.Application.Services.Cards.DTO_s
{
    public class CardUpdateRequest
    {
        public Guid WalletId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int ExpirationDay { get; set; }
        public int ClosingDay { get; set; }
        public string Type { get; set; }
    }
}
