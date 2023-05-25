using System.ComponentModel;

namespace FinanceControl.Application.Extensions.Enum;

public enum Message
{
    [Description("Adicionado com sucesso")]
    SUCCESSFULLY_ADDED,

    [Description("Atualizado com sucesso")]
    SUCCESSFULLY_UPDATED,

    [Description("Excluido com sucesso")]
    SUCCESSFULLY_DELETED,

    [Description("Objeto Inválido")]
    INVALID_OBJECT,

    [Description("Lista vazia")]
    LIST_EMPTY,

    [Description("Usuário não encontrado")]
    USER_NOT_FOUND,

    [Description("Usuário ou senha incorreta")]
    USER_PASSWORD_NOT_FOUND,

    [Description("Você não tem permissão")]
    USER_NOT_PERMISSION,

    [Description("Falha ao enviar email com seu código de validação. Tente novamente")]
    SEND_EMAIL_FAIL,

    [Description("O código de verificação foi enviado para o email cadastrado")]
    SEND_EMAIL_SUCCESS,

    [Description("Código Inválido")]
    CODE_INVALID
}