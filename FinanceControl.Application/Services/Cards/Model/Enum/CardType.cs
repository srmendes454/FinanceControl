using System.ComponentModel;

namespace FinanceControl.Application.Services.Cards.Model.Enum;

public enum CardType
{
    [Description("DEBIT")]
    DEBIT,

    [Description("CREDIT")]
    CREDIT,

    [Description("DEBIT_CREDIT")]
    DEBIT_CREDIT,
}