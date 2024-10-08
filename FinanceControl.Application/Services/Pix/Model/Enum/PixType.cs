using System.ComponentModel;

namespace FinanceControl.Application.Services.Pix.Model.Enum;

public enum PixType
{
    [Description("CPF")]
    CPF,

    [Description("Email")]
    EMAIL,

    [Description("Celular")]
    CELL_PHONE,

    [Description("Aleatória")]
    RANDOM
}