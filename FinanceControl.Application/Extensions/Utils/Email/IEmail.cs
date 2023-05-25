namespace FinanceControl.Application.Extensions.Utils.Email;

public interface IEmail
{
    bool Send(string email, string subject, string message);
}