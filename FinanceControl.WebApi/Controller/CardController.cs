using FinanceControl.Application.Services.Cards.DTO_s;
using FinanceControl.Application.Services.Cards.Service;
using FinanceControl.Application.Services.Wallet.Service;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.ControllerBase;
using FinanceControl.Infra.RequestContainer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Controller;

public class CardController : BaseController<CardController>
{
    #region [ Fields ]

    private readonly ICardService _service;

    #endregion

    #region [ Contructor ]
    public CardController(IAppSettings appSettings, ICardService service) : base(appSettings)
    {
        _service = service;
    }
    #endregion

    #region [ Public Routes ]

    /// <summary>
    /// Insere um cartão
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/v1/card")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertCard([FromBody] CardInsertRequest request)
    {
        return Ok(await _service.InsertCard(request));
    }

    /// <summary>
    /// Obtém os dados de um cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpGet("/v1/card/{cardId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid cardId)
    {
        return Ok(await _service.GetById(cardId));
    }

    /// <summary>
    /// Obtém todos os cartões
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="search"></param>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    [HttpGet("/v1/card/wallet/{walletId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromRoute] Guid walletId, [FromQuery] string search = null, [FromQuery] int take = 20, [FromQuery] int skip = 1)
    {
        return Ok(await _service.GetAll(walletId, search, take, skip));
    }

    /// <summary>
    /// Atualiza os dados de um cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("/v1/card/{cardId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid cardId, [FromBody] CardUpdateRequest request)
    {
        return Ok(await _service.Update(cardId, request));
    }

    /// <summary>
    /// Ativa um Cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    [HttpPut("/v1/card/{cardId}/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Active([FromRoute] Guid cardId)
    {
        return Ok(await _service.Active(cardId));
    }

    /// <summary>
    /// Inativa um Cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    [HttpPut("/v1/card/{cardId}/inactive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Inactive([FromRoute] Guid cardId)
    {
        return Ok(await _service.Inactive(cardId));
    }

    /// <summary>
    /// Exclui um cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpDelete("/v1/card/{cardId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid cardId)
    {
        return Ok(await _service.Delete(cardId));
    }

    #region [ List Enuns ]

    /// <summary>
    /// Lista todos os tipos de Cartões
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/card/type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ListCardTypes()
    {
        return Ok(_service.ListCardTypes());
    }

    #endregion

    #endregion
}