using System.ComponentModel;

namespace FinanceControl.Application.Services.Transactions.Model.Enum;

public enum TransactionsType
{
    [Description("CREDIT_CARD")]
    CREDIT_CARD,

    [Description("DEBIT_CARD")]
    DEBIT_CARD,

    [Description("BANK_SLIP")]
    BANK_SLIP,

    [Description("PIX")]
    PIX,

    [Description("BANK_TRANSFER")]
    BANK_TRANSFER,

    [Description("WITHDRAW_MONEY")]
    WITHDRAW_MONEY,

    [Description("INVESTMENT")]
    INVESTMENT
}