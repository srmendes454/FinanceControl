using System.ComponentModel;

namespace FinanceControl.Application.Extensions.Enum;

public enum Message
{
    [Description("adicionado com sucesso")]
    SUCCESSFULLY_ADDED,

    [Description("atualizado com sucesso")]
    SUCCESSFULLY_UPDATED,

    [Description("excluido com sucesso")]
    SUCCESSFULLY_DELETED,

    [Description("Objeto Inválido")]
    INVALID_OBJECT,

    [Description("Lista vazia")]
    LIST_EMPTY,

    [Description("Usuário não encontrado")]
    USER_NOT_FOUND,

    [Description("Falha ao enviar email com seu código de validação. Tente novamente")]
    SEND_EMAIL_FAIL,

    [Description("O código de verificação foi enviado para o email cadastrado")]
    SEND_EMAIL_SUCCESS,

    [Description("Código Inválido")]
    CODE_INVALID
}