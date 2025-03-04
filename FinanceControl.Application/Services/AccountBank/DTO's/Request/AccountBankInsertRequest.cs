using System;

namespace FinanceControl.Application.Services.AccountBank.DTO_s.Request
{
    public class AccountBankInsertRequest
    {
        public Guid WalletId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
    }
}
