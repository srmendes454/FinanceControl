using System.ComponentModel;

namespace FinanceControl.Application.Services.Cards.Model.Enum;

public enum CardType
{
    [Description("Débito")]
    DEBIT,

    [Description("Crédito")]
    CREDIT,

    [Description("Débito/Crédito")]
    DEBIT_CREDIT,
}