using System;
using System.Linq;
using FinanceControl.Application.Extensions.Session;
using FinanceControl.Extensions.AppSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace FinanceControl.Application.Extensions.ControllerBase;

[Authorize]
[Produces("application/json")]
public abstract class BaseController : Microsoft.AspNetCore.Mvc.ControllerBase
{
    #region [ Fields ]

    public ILogger _logger;

    public SessionBaseService _sessionService { get; set; }

    public readonly IAppSettings _appSettings;
    #endregion

    #region [ Constructor ]
    public BaseController(IAppSettings appSettings) : base()
    {
        _appSettings = appSettings;
        _logger = appSettings.GetLogger().ForContext<BaseController>();
        _sessionService = new SessionBaseService(appSettings: _appSettings, logger: _logger, currentUserId: GetCurrentUserId());
    }
    #endregion

    #region [ Public Methods ]
    public IActionResult AuthenticatorInfo()
    {
        try
        {
            var tokenAuthTimeLeft = HttpContext.Items
                .FirstOrDefault(p => p.Key.Equals("tokenAuthTimeLeft"))
                .Value;

            return Ok(tokenAuthTimeLeft);
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                dt = DateTime.UtcNow,
                ip = HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                au = 0,
                exMessage = ex.Message,
                exStackTrace = ex.StackTrace,
                exInnerException = ex.InnerException,
            });
        }
    }
    public Guid GetCurrentUserId()
    {
        var userId = HttpContext?.Items?.Where(p => p.Key.Equals("userId"))?.FirstOrDefault().Value;
        return userId == null ? Guid.Empty : Guid.Parse(userId.ToString());
    }
    
    #endregion
}