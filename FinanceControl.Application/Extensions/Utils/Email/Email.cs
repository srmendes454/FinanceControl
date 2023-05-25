using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace FinanceControl.Application.Extensions.Utils.Email;

public class Email : IEmail
{
    private readonly IConfiguration _configuration;

    public Email(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool Send(string email, string subject, string message)
    {
        try
        {
            var host = _configuration.GetValue<string>("SMTP:Host");
            var name = _configuration.GetValue<string>("SMTP:Name");
            var userName = _configuration.GetValue<string>("SMTP:UserName");
            var password = _configuration.GetValue<string>("SMTP:Password");
            var port = _configuration.GetValue<int>("SMTP:Port");

            var mail = new MailMessage
            {
                From = new MailAddress(userName, name),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
                Priority = MailPriority.High
            };

            mail.To.Add(email);

            var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = true
            };

            smtp.Send(mail);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}