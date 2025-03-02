using System.ComponentModel;

namespace FinanceControl.Domain.Enuns;

public enum TransactionsType
{
    [Description("Crédito")]
    CREDIT_CARD,

    [Description("Débito")]
    DEBIT_CARD,

    [Description("Boleto")]
    BANK_SLIP,

    [Description("PIX")]
    PIX,

    [Description("TED")]
    BANK_TRANSFER,

    [Description("Saque")]
    WITHDRAW
}