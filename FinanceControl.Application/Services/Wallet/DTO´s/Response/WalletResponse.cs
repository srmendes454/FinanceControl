using System;

namespace FinanceControl.Wallet.DTO_s.Response;

public class WalletResponse
{
    public Guid WalletId { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}