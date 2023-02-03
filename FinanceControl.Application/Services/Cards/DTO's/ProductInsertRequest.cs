using System;

namespace FinanceControl.Cards.DTO_s;

public class ProductInsertRequest
{
    public string Name { get; set; }
    public int NumberInstallments { get; set; }
    public double PriceInstallment { get; set; }
    public DateTime DatePurchase { get; set; }
}