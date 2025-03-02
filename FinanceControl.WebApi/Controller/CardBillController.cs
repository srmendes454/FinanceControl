using FinanceControl.Application.Services.CardBill.DTO_s.Request;
using FinanceControl.Application.Services.CardBill.Service;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.ControllerBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Controller;

public class CardBillController : BaseController<CardBillController>
{
    #region [ Fields ]

    private readonly ICardBillService _service;

    #endregion

    #region [ Contructor ]
    public CardBillController(IAppSettings appSettings, ICardBillService service) : base(appSettings)
    {
        _service = service;
    }
    #endregion

    #region [ Public Routes ]

    /// <summary>
    /// Gera a Fatura
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    [HttpPost("/v1/card-bill/card/{cardId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateCardBill([FromRoute] Guid cardId, [FromQuery] int month, [FromQuery] int year)
    {
        return Ok(await _service.GenerateCardBill(cardId, month, year));
    }

    /// <summary>
    /// Atualiza os dados da Fatura
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    [HttpPut("/v1/card-bill/{cardBillId}/card/{cardId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCardBill([FromRoute] Guid cardId, [FromRoute] Guid cardBillId, [FromQuery] int month, [FromQuery] int year)
    {
        return Ok(await _service.UpdateCardBill(cardId, cardBillId, month, year));
    }

    /// <summary>
    /// Atualiza os dados da Fatura
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    [HttpPost("/v1/card-bill/{cardBillId}/pay")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PayCardBill([FromRoute] Guid cardBillId, [FromBody] PayCardBillRequest request)
    {
        return Ok(await _service.PayCardBill(cardBillId, request));
    }

    #endregion
}