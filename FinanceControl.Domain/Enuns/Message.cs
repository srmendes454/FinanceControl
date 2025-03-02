using System.ComponentModel;

namespace FinanceControl.Domain.Enuns;

public enum Message
{
    [Description("adicionado com sucesso")]
    SUCCESSFULLY_ADDED_M,

    [Description("adicionada com sucesso")]
    SUCCESSFULLY_ADDED_F,

    [Description("atualizado com sucesso")]
    SUCCESSFULLY_UPDATED_M,

    [Description("atualizada com sucesso")]
    SUCCESSFULLY_UPDATED_F,

    [Description("excluido com sucesso")]
    SUCCESSFULLY_DELETED_M,

    [Description("excluida com sucesso")]
    SUCCESSFULLY_DELETED_F,

    [Description("Objeto Inválido")]
    INVALID_OBJECT,

    [Description("Lista vazia")]
    LIST_EMPTY,

    [Description("Usuário não encontrado")]
    USER_NOT_FOUND
}