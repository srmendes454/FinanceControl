using FinanceControl.Application.Extensions.ControllerBase;
using FinanceControl.Application.Extensions.RequestContainer;
using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Application.Services.Transactions.Service;
using FinanceControl.Extensions.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.Utils.Email;

namespace FinanceControl.Controller;

public class TransactionsController : BaseController
{
    private readonly IRequestContainer _request;
    private readonly IEmail _email;
    #region [ Constructor ]

    public TransactionsController(IAppSettings appSettings, IRequestContainer request, IEmail email) : base(appSettings)
    {
        _logger = appSettings.GetLogger().ForContext<TransactionsController>();
        _request = request;
        _email = email;
    }

    #endregion

    #region [ Public Routes ]

    /// <summary>
    /// Insere uma Transação
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/v1/transaction")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Insert([FromBody] TransactionsInsertRequest request)
    {
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email);
        return Ok(await service.Insert(request));
    }

    #endregion
}