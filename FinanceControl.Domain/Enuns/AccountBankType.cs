using System.ComponentModel;

namespace FinanceControl.Domain.Enuns;

public enum AccountBankType
{
    [Description("Conta Corrente")]
    CURRENT_ACCOUNT,

    [Description("Conta Poupança")]
    SAVINGS_ACCOUNT,

    [Description("Conta Salário")]
    SALARY_ACCOUNT,

    [Description("Conta PJ")]
    PJ_ACCOUNT
}