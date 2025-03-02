using System.ComponentModel;

namespace FinanceControl.Domain.Enuns;

public enum PixType
{
    [Description("CPF")]
    CPF,

    [Description("CNPJ")]
    CNPJ,

    [Description("Email")]
    EMAIL,

    [Description("Celular")]
    CELL_PHONE,

    [Description("Aleatória")]
    RANDOM
}