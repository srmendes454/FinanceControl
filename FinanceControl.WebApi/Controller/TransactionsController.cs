using FinanceControl.Application.Extensions.ControllerBase;
using FinanceControl.Application.Extensions.RequestContainer;
using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Application.Services.Transactions.Service;
using FinanceControl.Extensions.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.Utils.Email;
using System;
using FinanceControl.Application.Extensions.Utils.Repetition;
using FinanceControl.Application.Extensions.Utils.SignedBy;

namespace FinanceControl.Controller;

public class TransactionsController : BaseController
{
    private readonly IRequestContainer _request;
    private readonly IEmail _email;
    private readonly IAddRepetition _addRepetition;
    private readonly ISignedBy _signedBy;

    #region [ Constructor ]

    public TransactionsController(IAppSettings appSettings, IRequestContainer request, IEmail email, IAddRepetition addRepetition, ISignedBy signedBy) : base(appSettings)
    {
        _logger = appSettings.GetLogger().ForContext<TransactionsController>();
        _request = request;
        _email = email;
        _addRepetition = addRepetition;
        _signedBy = signedBy;
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
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email, _addRepetition, _signedBy);
        return Ok(await service.Insert(request));
    }

    /// <summary>
    /// Lista todas as transações por tipo de Pagamento e outros filtros
    /// </summary>
    /// <param name="paymentId"></param>
    /// <returns></returns>
    [HttpGet("/v1/transaction/paymentId/{paymentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllByPaymentId([FromRoute] Guid paymentId, [FromQuery] Guid assignedId, [FromQuery] string search = null, [FromQuery] string type = null, [FromQuery] int year = 0, [FromQuery] int month = 0, [FromQuery] int take = 20, [FromQuery] int skip = 1)
    {
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email, _addRepetition, _signedBy);
        return Ok(await service.GetAllByPaymentId(paymentId, assignedId, search, type, year, month, take, skip));
    }

    /// <summary>
    /// Lista uma Transação por Id e pela Data
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpGet("/v1/transaction/{transactionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAndDate([FromRoute] Guid transactionId, [FromQuery] int year = 0, [FromQuery] int month = 0)
    {
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email, _addRepetition, _signedBy);
        return Ok(await service.GetByIdAndDate(transactionId, year, month));
    }

    /// <summary>
    /// Atualiza os dados de uma transação
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpPut("/v1/transaction/{transactionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] TransactionsUpdateRequest request, [FromRoute] Guid transactionId, [FromQuery] int year = 0, [FromQuery] int month = 0)
    {
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email, _addRepetition, _signedBy);
        return Ok(await service.Update(transactionId, year, month, request));
    }

    /// <summary>
    /// Deleta uma transação
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpDelete("/v1/transaction/{transactionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid transactionId, [FromQuery] int year = 0, [FromQuery] int month = 0)
    {
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email, _addRepetition, _signedBy);
        return Ok(await service.Delete(transactionId, year, month));
    }

    #region [ Assigned Transaction ]

    /// <summary>
    /// Obtem todas as Transações que me foi atribuido
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("/v1/transaction/marked")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAssignedTransactions(string search, int take = 20, int skip = 1)
    {
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email, _addRepetition, _signedBy);
        return Ok(await service.ListAssignedTransactions(search, take, skip));
    }

    /// <summary>
    /// Serviço para Avaliar as atribuições em transações
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("/v1/transaction/evaluate-assigned")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EvaluateAssignedTransaction([FromBody] TransactionsEvaluateAssignedRequest request)
    {
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email, _addRepetition, _signedBy);
        return Ok(await service.EvaluateAssignedTransaction(request));
    }

    #endregion

    #region [ List Enuns ]

    /// <summary>
    /// Lista todos as opções de Tipos de Despesa
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/transaction/expense-type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ListExpenseType()
    {
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email, _addRepetition, _signedBy);
        return Ok(service.ListExpenseType());
    }

    /// <summary>
    /// Lista todas as opções de Fluxo de Caixa
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/transaction/cash-flow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ListCashFlow()
    {
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email, _addRepetition, _signedBy);
        return Ok(service.ListCashFlow());
    }

    /// <summary>
    /// Lista todas as opções de Tipo de Transação
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/transaction/type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ListTransactionsType()
    {
        using var service = new TransactionsService(_appSettings, _logger, _request.UserId, _email, _addRepetition, _signedBy);
        return Ok(service.ListTransactionsType());
    }

    #endregion

    #endregion
}