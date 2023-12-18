using System;

namespace FinanceControl.Cards.DTO_s;

public class CardResponse
{
    public Guid CardId { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public int ExpirationDay { get; set; }
    public int ClosingDay { get; set; }
    public string Type { get; set; }
}