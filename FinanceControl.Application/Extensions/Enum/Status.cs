using System.ComponentModel;

namespace FinanceControl.Application.Extensions.Enum
{
    public enum Status
    {
        [Description("TO_WIN")]
        TO_WIN,

        [Description("OVERDUE")]
        OVERDUE,

        [Description("PAID_OUT")]
        PAID_OUT
    }
}