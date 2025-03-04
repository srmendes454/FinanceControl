using System.ComponentModel;

namespace FinanceControl.Domain.Enuns;

public enum InvestmentType
{
    [Description("Poupança")]
    SAVINGS,

    [Description("Tesouro Direto")]
    TREASURY_DIRECT,

    [Description("CDB's")]
    CDB,

    [Description("Ações")]
    ACTIONS,

    [Description("Fundo de Investimento Fixo")]
    FIXED_INVESTMENT,

    [Description("Fundo de Investimento Variável")]
    VARIABLE_INVESTMENT
}