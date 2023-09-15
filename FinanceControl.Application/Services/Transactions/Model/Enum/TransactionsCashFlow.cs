using System.ComponentModel;

namespace FinanceControl.Application.Services.Transactions.Model.Enum;

public enum TransactionsCashFlow
{
    [Description("Entrada")]
    ENTRY,

    [Description("Saida")]
    EXIT,
}