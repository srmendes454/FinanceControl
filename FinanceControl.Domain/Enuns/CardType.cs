using System.ComponentModel;

namespace FinanceControl.Domain.Enuns;

public enum CardType
{
    [Description("Débito")]
    DEBIT,

    [Description("Crédito")]
    CREDIT,

    [Description("Débito/Crédito")]
    DEBIT_CREDIT
}