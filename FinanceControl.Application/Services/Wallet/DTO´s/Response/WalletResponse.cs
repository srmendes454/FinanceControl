using System;

namespace FinanceControl.Application.Services.Wallet.DTO_s.Response;

public class WalletResponse
{
    public Guid WalletId { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public int ReceiptDay { get; set; }
    public double Income { get; set; }
}