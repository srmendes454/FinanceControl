using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Application.Services.Transactions.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.Utils.Email;
using System;
using FinanceControl.Application.Extensions.Utils.Repetition;
using FinanceControl.Application.Extensions.Utils.SignedBy;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.ControllerBase;
using FinanceControl.Infra.RequestContainer;

namespace FinanceControl.Controller;

public class TransactionsController : BaseController<TransactionsController>
{
    #region [ Fields ]

    private readonly ITransactionsService _service;

    #endregion

    #region [ Constructor ]

    public TransactionsController(IAppSettings appSettings, ITransactionsService service) : base(appSettings)
    {
        _service = service;
    }

    #endregion

    #region [ Public Routes ]

    /// <summary>
    /// Insere uma Transação por Cartão
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/v1/transaction/card/{cardId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertToCard([FromRoute] Guid cardId, [FromBody] TransactionsInsertRequest request)
    {
        return Ok(await _service.InsertToCard(cardId, request));
    }

    /// <summary>
    /// Insere uma Transação por Boleto
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/v1/transaction/bank-slip/{bankSlipId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertToBankSlip([FromRoute] Guid bankSlipId, [FromBody] TransactionsInsertRequest request)
    {
        return Ok(await _service.InsertToBankSlip(bankSlipId, request));
    }

    /// <summary>
    /// Insere uma Transação por Boleto
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/v1/transaction/pix/{pixId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertToPix([FromRoute] Guid pixId, [FromBody] TransactionsInsertRequest request)
    {
        return Ok(await _service.InsertToPix(pixId, request));
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
        return Ok(await _service.GetAllByPaymentId(paymentId, assignedId, search, type, year, month, take, skip));
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
        return Ok(await _service.GetByIdAndDate(transactionId, year, month));
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
        return Ok(await _service.Update(transactionId, year, month, request));
    }

    /// <summary>
    /// Move as Transações
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpPut("/v1/transaction/{transactionId}/move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid transactionId, [FromQuery] bool next)
    {
        return Ok(await _service.MoveTransaction(transactionId, next));
    }

    /// <summary>
    /// Deleta uma transação
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpDelete("/v1/transaction/{transactionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid transactionId, [FromQuery] int year = 0, [FromQuery] int month = 0, [FromQuery] bool deleteAll = false)
    {
        return Ok(await _service.Delete(transactionId, year, month, deleteAll));
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
        return Ok(await _service.ListAssignedTransactions(search, take, skip));
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
        return Ok(await _service.EvaluateAssignedTransaction(request));
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
        return Ok(_service.ListExpenseType());
    }

    /// <summary>
    /// Lista todas as opções de Fluxo de Caixa
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/transaction/cash-flow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ListCashFlow()
    {
        return Ok(_service.ListCashFlow());
    }

    /// <summary>
    /// Lista todas as opções de Tipo de Transação
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/transaction/type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ListTransactionsType()
    {
        return Ok(_service.ListTransactionsType());
    }

    #endregion

    #endregion
}