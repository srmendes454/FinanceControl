using System.ComponentModel;

namespace FinanceControl.Extensions.Enum;

public enum Message
{
    [Description("SUCCESSFULLY ADDED")]
    SUCCESSFULLY_ADDED,

    [Description("SUCCESSFULLY UPDATED")]
    SUCCESSFULLY_UPDATED,

    [Description("SUCCESSFULLY DELETED")]
    SUCCESSFULLY_DELETED,

    [Description("INVALID OBJECT")]
    INVALID_OBJECT,

    [Description("LIST EMPTY")]
    LIST_EMPTY,

    [Description("USER NOT FOUND")]
    USER_NOT_FOUND,

    [Description("USER NOT PERMISSION")]
    USER_NOT_PERMISSION
}