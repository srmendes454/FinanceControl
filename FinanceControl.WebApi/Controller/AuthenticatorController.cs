using FinanceControl.Application.Extensions.ControllerBase;
using FinanceControl.Extensions.AppSettings;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Controller;

public class AuthenticatorController : BaseController
{
    #region [ Constructor ]

    public AuthenticatorController(IAppSettings appSettings) : base(appSettings)
    {
        _logger = appSettings.GetLogger().ForContext<AuthenticatorController>();
    }

    #endregion

    #region [ Public Routes ]

    [HttpGet("/authenticator-info")]
    public IActionResult AuthAuthenticatorInfo() => AuthenticatorInfo();

    #endregion
}