using System;

namespace FinanceControl.Application.Services.Cards.DTO_s;

public class CardInsertRequest
{
    public Guid WalletId { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public string Color { get; set; }
    public int DateExpiration { get; set; }
    public double AvailableLimit { get; set; }
    public string CardType { get; set; }
}