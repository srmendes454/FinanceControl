using System;

namespace FinanceControl.Application.Services.Wallet.DTO_s.Response;

public class OptimizeIncomeResponse
{
    public Guid OptimizeIncomeId { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public int Percent { get; set; }
}