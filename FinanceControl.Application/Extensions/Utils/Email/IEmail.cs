namespace FinanceControl.Application.Extensions.Utils.Email;

public interface IEmail
{
    bool Send(string email, string subject, string message);

    string TemplateResetPassword(string name, string subject, string message, string code);

    string TemplateWelcome(string name, string subject, string message);    
}