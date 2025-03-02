using FinanceControl.Infra.AppSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace FinanceControl.Infra.ControllerBase;

[Authorize]
[Produces("application/json")]
public abstract class BaseController<T> : Microsoft.AspNetCore.Mvc.ControllerBase
{
    #region [ Fields ]

    public ILogger _logger;
    public readonly IAppSettings _appSettings;

    #endregion

    #region [ Constructor ]
    public BaseController(IAppSettings appSettings) : base()
    {
        _appSettings = appSettings;
        _logger = appSettings.GetLogger().ForContext<BaseController<T>>();
    }
    #endregion

    #region [ Public Methods ]

    #endregion
}