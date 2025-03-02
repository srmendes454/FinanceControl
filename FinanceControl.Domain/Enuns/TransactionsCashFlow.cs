using System.ComponentModel;

namespace FinanceControl.Domain.Enuns;

public enum TransactionsCashFlow
{
    [Description("Entrada")]
    ENTRY,

    [Description("Saida")]
    EXIT,
}