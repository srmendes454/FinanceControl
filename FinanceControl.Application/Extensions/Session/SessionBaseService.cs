using System;
using System.Linq;
using FinanceControl.Extensions.AppSettings;
using FinanceControl.WebApi.Extensions.Session;
using ILogger = Serilog.ILogger;

namespace FinanceControl.Application.Extensions.Session;

public class SessionBaseService : BaseService.BaseService
{
    public SessionBaseService(IAppSettings appSettings, ILogger logger,
        Guid currentUserId) : base(logger: logger, appSettings: appSettings,
        currentUserId: currentUserId)
    {
    }

    public SessionBaseRequest GetSessionDto()
    {
        var httpItems = _appSettings.GetHttpContext()?.HttpContext?.Items;
        var userId = httpItems?.Where(p => p.Key.Equals("userId"))?.FirstOrDefault().Value?.ToString();

        var result = new SessionBaseRequest
        {
            AuthenticatedUserId = Guid.Parse(userId)
        };

        return result;
    }
}