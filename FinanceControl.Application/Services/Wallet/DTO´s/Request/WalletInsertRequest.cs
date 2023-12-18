namespace FinanceControl.Application.Services.Wallet.DTO_s.Request;

public class WalletInsertRequest
{
    public string Name { get; set; }
    public string Color { get; set; }
    public int ReceiptDay { get; set; }
    public double Income { get; set; }
}