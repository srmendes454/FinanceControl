using System.ComponentModel;

namespace FinanceControl.Application.Services.Transactions.Model.Enum;

public enum TransactionsCashFlow
{
    [Description("ENTRY")]
    ENTRY,

    [Description("EXIT")]
    EXIT,
}