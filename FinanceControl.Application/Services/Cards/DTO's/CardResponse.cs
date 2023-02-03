using System;

namespace FinanceControl.Cards.DTO_s;

public class CardResponse
{
    public Guid CardId { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public string Color { get; set; }
    public DateTime DateExpiration { get; set; }
    public DateTime ClosingDay { get; set; }
    public bool Payment { get; set; }
    public double PriceTotal { get; set; }
    public double AvailableLimit { get; set; }
    public string Status { get; set; }
    public string CardType { get; set; }
}