using System;

namespace FinanceControl.Application.Services.Cards.DTO_s;

public class CardInsertRequest
{
    public Guid WalletId { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public int DateExpiration { get; set; }
    public string Type { get; set; }
}