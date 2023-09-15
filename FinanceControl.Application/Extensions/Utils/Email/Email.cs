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

    private const string ContactUs = "Caso tenha alguma dúvida, entre em contato conosco ";
    private const string RegisterMessage = "Caso queira Gerenciar suas finanças de forma gratuita, cadastre-se na nossa plataforma";

    public bool Send(string email, string subject, string template)
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
                Body = template,
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

    public string TemplateResetPassword(string name, string subject, string message, string code)
    {
        var logo = "";
        var template = $@"
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout:fixed;background-color:#2D332D"" id=""bodyTable"">
	            <tbody>
		            <tr>
			            <td style=""padding-right:10px;padding-left:10px;"" align=""center"" valign=""top"" id=""bodyCell"">
				            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""wrapperWebview"" style=""max-width:600px"">
					            <tbody>
						            <tr>
							            <td align=""center"" valign=""top"">
								            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
									            <tbody>
										            <tr>
											            <td style=""padding-top: 20px; padding-bottom: 20px; padding-right: 0px;"" align=""right"" valign=""middle"" class=""webview"">
											            </td>
										            </tr>
									            </tbody>
								            </table>
							            </td>
						            </tr>
					            </tbody>
				            </table>
				            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""wrapperBody"" style=""max-width:600px"">
					            <tbody>
						            <tr>
							            <td align=""center"" valign=""top"">
								            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""tableCard"" style=""background-color:#2D332D;border-color:#232623;border-style:solid;border-width:0 2px 2px 2px;"">
									            <tbody>
										            <tr>
											            <td style=""background-color:#2C7333;font-size:1px;line-height:4px"" class=""topBorder"" height=""3"">&nbsp;</td>
										            </tr>
										            <tr>
											            <td style=""padding-top: 60px; padding-bottom: 20px;"" align=""center"" valign=""middle"" class=""emailLogo"">
												            <a href=""#"" style=""text-decoration:none"" target=""_blank"">
													            <img alt="""" border=""0"" src=""{logo}"" style=""width:100%;max-width:150px;height:auto;display:block"" width=""150"">
												            </a>
											            </td>
										            </tr>
										            <tr>
											            <td style=""padding-bottom: 5px; padding-left: 20px; padding-right: 20px;"" align=""center"" valign=""top"" class=""mainTitle"">
												            <h2 class=""text"" style=""color:#FFF;font-family:Poppins,Helvetica,Arial,sans-serif;font-size:28px;font-weight:500;font-style:normal;letter-spacing:normal;line-height:36px;text-transform:none;text-align:center;padding:0;margin:0"">Olá {name}</h2>
											            </td>
										            </tr>
										            <tr>
											            <td style=""padding-bottom: 30px; padding-left: 20px; padding-right: 20px;"" align=""center"" valign=""top"" class=""subTitle"">
												            <h4 class=""text"" style=""color:#999;font-family:Poppins,Helvetica,Arial,sans-serif;font-size:16px;font-weight:500;font-style:normal;letter-spacing:normal;line-height:24px;text-transform:none;text-align:center;padding:0;margin:0"">{subject}</h4>
											            </td>
										            </tr>
										            <tr>
											            <td style=""padding-left:20px;padding-right:20px"" align=""center"" valign=""top"" class=""containtTable ui-sortable"">
												            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""tableDescription"" style="""">
													            <tbody>
														            <tr>
															            <td style=""padding-bottom: 20px;"" align=""center"" valign=""top"" class=""description"">
																            <p class=""text"" style=""color:#666;font-family:'Open Sans',Helvetica,Arial,sans-serif;font-size:14px;font-weight:400;font-style:normal;letter-spacing:normal;line-height:22px;text-transform:none;text-align:center;padding:0;margin:0"">{message}</p>
															            </td>
														            </tr>
													            </tbody>
												            </table>
												            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""tableButton"" style="""">
													            <tbody>
														            <tr>
															            <td style=""padding-top:20px;padding-bottom:20px"" align=""center"" valign=""top"">
																            <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"">
																	            <tbody>
																		            <tr>
																			            <td style=""background-color: #2C7333; padding: 10px 35px; border-radius: 50px;"" align=""center"" class=""ctaButton""> 
                                                                                            <h2 class=""text"" style=""color:#fff;font-family:Poppins,Helvetica,Arial,sans-serif;font-size:13px;font-weight:600;font-style:normal;letter-spacing:1px;line-height:20px;text-transform:uppercase;text-decoration:none;display:block"">{code}</h2>
																			            </td>
																		            </tr>
																	            </tbody>
																            </table>
															            </td>
														            </tr>
													            </tbody>
												            </table>
											            </td>
										            </tr>
									            </tbody>
								            </table>
								            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""space"">
									            <tbody>
										            <tr>
											            <td style=""font-size:1px;line-height:1px"" height=""30"">&nbsp;</td>
										            </tr>
									            </tbody>
								            </table>
							            </td>
						            </tr>
					            </tbody>
				            </table>
				            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""wrapperFooter"" style=""max-width:600px"">
					            <tbody>
						            <tr>
							            <td align=""center"" valign=""top"">
								            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""footer"">
									            <tbody>
										            <tr>
											            <td style=""padding: 0px 10px 10px;"" align=""center"" valign=""top"" class=""footerEmailInfo"">
												            <p class=""text"" style=""color:#bbb;font-family:'Open Sans',Helvetica,Arial,sans-serif;font-size:12px;font-weight:400;font-style:normal;letter-spacing:normal;line-height:20px;text-transform:none;text-align:center;padding:0;margin:0"">{ContactUs}<a href=""#"" style=""color:#bbb;text-decoration:underline"" target=""_blank"">support@mail.com</a>
													   </td>
										            </tr>
									            </tbody>
								            </table>
							            </td>
						            </tr>
						            <tr>
							            <td style=""font-size:1px;line-height:1px"" height=""30"">&nbsp;</td>
						            </tr>
					            </tbody>
				            </table>
			            </td>
		            </tr>
	            </tbody>
            </table>";
        return template;
    }

    public string TemplateWelcome(string name, string subject, string message)
    {
        var logo = "";
        var template = $@"
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout:fixed;background-color:#2D332D"" id=""bodyTable"">
	            <tbody>
		            <tr>
			            <td style=""padding-right:10px;padding-left:10px;"" align=""center"" valign=""top"" id=""bodyCell"">
				            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""wrapperWebview"" style=""max-width:600px"">
					            <tbody>
						            <tr>
							            <td align=""center"" valign=""top"">
								            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
									            <tbody>
										            <tr>
											            <td style=""padding-top: 20px; padding-bottom: 20px; padding-right: 0px;"" align=""right"" valign=""middle"" class=""webview"">
											            </td>
										            </tr>
									            </tbody>
								            </table>
							            </td>
						            </tr>
					            </tbody>
				            </table>
				            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""wrapperBody"" style=""max-width:600px"">
					            <tbody>
						            <tr>
							            <td align=""center"" valign=""top"">
								            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""tableCard"" style=""background-color:#2D332D;border-color:#232623;border-style:solid;border-width:0 2px 2px 2px;"">
									            <tbody>
										            <tr>
											            <td style=""background-color:#2C7333;font-size:1px;line-height:4px"" class=""topBorder"" height=""3"">&nbsp;</td>
										            </tr>
										            <tr>
											            <td style=""padding-top: 60px; padding-bottom: 20px;"" align=""center"" valign=""middle"" class=""emailLogo"">
												            <a href=""#"" style=""text-decoration:none"" target=""_blank"">
													            <img alt="""" border=""0"" src=""{logo}"" style=""width:100%;max-width:150px;height:auto;display:block"" width=""150"">
												            </a>
											            </td>
										            </tr>
										            <tr>
											            <td style=""padding-bottom: 5px; padding-left: 20px; padding-right: 20px;"" align=""center"" valign=""top"" class=""mainTitle"">
												            <h2 class=""text"" style=""color:#FFF;font-family:Poppins,Helvetica,Arial,sans-serif;font-size:28px;font-weight:500;font-style:normal;letter-spacing:normal;line-height:36px;text-transform:none;text-align:center;padding:0;margin:0"">Olá {name}</h2>
											            </td>
										            </tr>
										            <tr>
											            <td style=""padding-bottom: 30px; padding-left: 20px; padding-right: 20px;"" align=""center"" valign=""top"" class=""subTitle"">
												            <h4 class=""text"" style=""color:#999;font-family:Poppins,Helvetica,Arial,sans-serif;font-size:16px;font-weight:500;font-style:normal;letter-spacing:normal;line-height:24px;text-transform:none;text-align:center;padding:0;margin:0"">{subject}</h4>
											            </td>
										            </tr>
										            <tr>
											            <td style=""padding-left:20px;padding-right:20px"" align=""center"" valign=""top"" class=""containtTable ui-sortable"">
												            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""tableDescription"" style="""">
													            <tbody>
														            <tr>
															            <td style=""padding-bottom: 20px;"" align=""center"" valign=""top"" class=""description"">
																            <p class=""text"" style=""color:#666;font-family:'Open Sans',Helvetica,Arial,sans-serif;font-size:14px;font-weight:400;font-style:normal;letter-spacing:normal;line-height:22px;text-transform:none;text-align:center;padding:0;margin:0"">{message}</p>
															            </td>
														            </tr>
													            </tbody>
												            </table>
												            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""tableButton"" style="""">
													            <tbody>
														            <tr>
															            <td style=""padding-top:20px;padding-bottom:20px"" align=""center"" valign=""top"">
																            <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"">
																	            <tbody>
																		            <tr>
																			            <td style=""background-color: #2C7333; padding: 10px 35px; border-radius: 50px;"" align=""center"" class=""ctaButton""> 
                                                                                            <a class=""text"" style=""color:#fff;font-family:Poppins,Helvetica,Arial,sans-serif;font-size:13px;font-weight:600;font-style:normal;letter-spacing:1px;line-height:20px;text-transform:uppercase;text-decoration:none;display:block"" href=""http://localhost:3000/login"">LOGIN</a>
																			            </td>
																		            </tr>
																	            </tbody>
																            </table>
															            </td>
														            </tr>
													            </tbody>
												            </table>
											            </td>
										            </tr>
									            </tbody>
								            </table>
								            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""space"">
									            <tbody>
										            <tr>
											            <td style=""font-size:1px;line-height:1px"" height=""30"">&nbsp;</td>
										            </tr>
									            </tbody>
								            </table>
							            </td>
						            </tr>
					            </tbody>
				            </table>
				            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""wrapperFooter"" style=""max-width:600px"">
					            <tbody>
						            <tr>
							            <td align=""center"" valign=""top"">
								            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""footer"">
									            <tbody>
										            <tr>
											            <td style=""padding: 0px 10px 10px;"" align=""center"" valign=""top"" class=""footerEmailInfo"">
												            <p class=""text"" style=""color:#bbb;font-family:'Open Sans',Helvetica,Arial,sans-serif;font-size:12px;font-weight:400;font-style:normal;letter-spacing:normal;line-height:20px;text-transform:none;text-align:center;padding:0;margin:0"">{ContactUs}<a href=""#"" style=""color:#bbb;text-decoration:underline"" target=""_blank"">support@mail.com</a>
													   </td>
										            </tr>
									            </tbody>
								            </table>
							            </td>
						            </tr>
						            <tr>
							            <td style=""font-size:1px;line-height:1px"" height=""30"">&nbsp;</td>
						            </tr>
					            </tbody>
				            </table>
			            </td>
		            </tr>
	            </tbody>
            </table>";
        return template;
    }

    public string TemplateTransactionNotification(string name, string nameTransaction, double valueTransaction, string namePayment, string typePayment)
    {
        var logo = "";
        var template = $@"
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout:fixed;background-color:#2D332D""
	                id=""bodyTable"">
	                <tbody>
		                <tr>
			                <td style=""padding-right:10px;padding-left:10px;"" align=""center"" valign=""top"" id=""bodyCell"">
				                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""wrapperWebview""
					                style=""max-width:600px"">
					                <tbody>
						                <tr>
							                <td align=""center"" valign=""top"">
								                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
									                <tbody>
										                <tr>
											                <td style=""padding-top: 20px; padding-bottom: 20px; padding-right: 0px;""
												                align=""right"" valign=""middle"" class=""webview"">
											                </td>
										                </tr>
									                </tbody>
								                </table>
							                </td>
						                </tr>
					                </tbody>
				                </table>
				                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""wrapperBody""
					                style=""max-width:600px"">
					                <tbody>
						                <tr>
							                <td align=""center"" valign=""top"">
								                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""tableCard""
									                style=""background-color:#2D332D;border-color:#232623;border-style:solid;border-width:0 2px 2px 2px;"">
									                <tbody>
										            <tr>
											            <td style=""background-color:#2C7333;font-size:1px;line-height:4px"" class=""topBorder"" height=""3"">&nbsp;</td>
										            </tr>
										            <tr>
											            <td style=""padding-top: 60px; padding-bottom: 20px;"" align=""center"" valign=""middle"" class=""emailLogo"">
												            <a href=""#"" style=""text-decoration:none"" target=""_blank"">
													            <img alt="""" border=""0"" src=""{logo}"" style=""width:100%;max-width:150px;height:auto;display:block"" width=""150"">
												            </a>
											            </td>
										            </tr>
										            <tr>
											                <td style=""padding-bottom: 5px; padding-left: 20px; padding-right: 20px;""
												                align=""center"" valign=""top"" class=""mainTitle"">
												                <h2 class=""text""
													                style=""color:#FFF;font-family:Poppins,Helvetica,Arial,sans-serif;font-size:28px;font-weight:500;font-style:normal;letter-spacing:normal;line-height:36px;text-transform:none;text-align:center;padding:0;margin:0"">
													                Olá</h2>
											                </td>
										                </tr>
										                <tr>
											                <td style=""padding-bottom: 30px; padding-left: 20px; padding-right: 20px;""
												                align=""center"" valign=""top"" class=""subTitle"">
												                <h4 class=""text""
													                style=""color:#999;font-family:Poppins,Helvetica,Arial,sans-serif;font-size:16px;font-weight:500;font-style:normal;letter-spacing:normal;line-height:24px;text-transform:none;text-align:center;padding:0;margin:0"">
													                O {name} marcou você nessa movimentação financeira</h4>
											                </td>
										                </tr>
										                <tr>
											                <td style=""padding-left:20px;padding-right:20px"" align=""center"" valign=""top""
												                class=""containtTable ui-sortable"">
												                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%""
													                class=""tableDescription"">
													                <tbody>
														                <tr>
															                <td style=""padding-bottom: 20px;"" align=""center""
																                valign=""top"" class=""description"">
																                <p class=""text""
																	                style=""color:#3E6943;font-family:Poppins,'Open Sans',Helvetica,Arial,sans-serif;font-size:16px;font-weight:700;font-style:normal;letter-spacing:normal;line-height:16px;text-transform:none;text-align:center;padding:0;margin:0"">
																	                Compra {nameTransaction} no valor de R$ {valueTransaction}
																	                via {namePayment}-{typePayment}
																                </p>
																                <br>
																                <p
																	                style=""color:#666;font-family:Poppins, 'Open Sans',Helvetica,Arial,sans-serif;font-size:14px;font-weight:500;font-style:normal;letter-spacing:normal;line-height:16px;text-transform:none;text-align:center;padding:0;margin:0"">
																	                {RegisterMessage}
																                </p>
															                </td>
														                </tr>
													                </tbody>
												                </table>
												                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%""
													                class=""tableButton"">
													                <tbody>
														                <tr>
															                <td style=""padding-top:20px;padding-bottom:20px""
																                align=""center"" valign=""top"">
																                <table border=""0"" cellpadding=""0"" cellspacing=""0""
																	                align=""center"">
																	                <tbody>
																		                <tr>
																			                <td style=""background-color: #2C7333; padding: 10px 35px; border-radius: 50px;""
																				                align=""center"" class=""ctaButton"">
																				                <a class=""text""
																					                style=""color:#fff;font-family:Poppins,Helvetica,Arial,sans-serif;font-size:13px;font-weight:600;font-style:normal;letter-spacing:1px;line-height:20px;text-transform:uppercase;text-decoration:none;display:block""
																					                href=""http://localhost:3000/register"">INSCREVER-SE</a>
																			                </td>
																		                </tr>
																	                </tbody>
																                </table>
															                </td>
														                </tr>
													                </tbody>
												                </table>
											                </td>
										                </tr>
									                </tbody>
								                </table>
								                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""space"">
									                <tbody>
										                <tr>
											                <td style=""font-size:1px;line-height:1px"" height=""30"">&nbsp;</td>
										                </tr>
									                </tbody>
								                </table>
							                </td>
						                </tr>
					                </tbody>
				                </table>
				                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""wrapperFooter""
					                style=""max-width:600px"">
					                <tbody>
						                <tr>
							                <td align=""center"" valign=""top"">
								                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""footer"">
									                <tbody>
										                <tr>
											                <td style=""padding: 0px 10px 10px;"" align=""center"" valign=""top""
												                class=""footerEmailInfo"">
												                <p class=""text""
													                style=""color:#bbb;font-family:'Open Sans',Helvetica,Arial,sans-serif;font-size:12px;font-weight:400;font-style:normal;letter-spacing:normal;line-height:20px;text-transform:none;text-align:center;padding:0;margin:0"">
													                {ContactUs} <a href=""#""
														                style=""color:#bbb;text-decoration:underline""
														                target=""_blank"">support@mail.com.</a>
											                </td>
										                </tr>
									                </tbody>
								                </table>
							                </td>
						                </tr>
						                <tr>
							                <td style=""font-size:1px;line-height:1px"" height=""30"">&nbsp;</td>
						                </tr>
					                </tbody>
				                </table>
			                </td>
		                </tr>
	                </tbody>
                </table>";
        return template;
    }
}