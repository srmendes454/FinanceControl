using FinanceControl.Application.Services.Cards.DTO_s;
using FinanceControl.Extensions.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.ControllerBase;
using FinanceControl.Application.Extensions.RequestContainer;
using FinanceControl.Application.Services.Cards.Service;

namespace FinanceControl.Controller;

public class CardController : BaseController
{
    private readonly IRequestContainer _request;
    #region [ Contructor ]
    public CardController(IAppSettings appSettings, IRequestContainer request) : base(appSettings)
    {
        _logger = appSettings.GetLogger().ForContext<CardController>();
        _request = request;
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
        using var service = new CardService(_appSettings, _logger, _request.UserId);
        return Ok(await service.InsertCard(request));
    }

    /// <summary>
    /// Obtém os dados de um cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpGet("/v1/card/{cardId}/wallet/{walletId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid cardId, [FromRoute] Guid walletId)
    {
        using var service = new CardService(_appSettings, _logger, GetCurrentUserId());
        return Ok(await service.GetById(walletId, cardId));
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
        using var service = new CardService(_appSettings, _logger, GetCurrentUserId());
        return Ok(await service.GetAll(walletId, search, take, skip));
    }

    /// <summary>
    /// Atualiza os dados de um cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("/v1/card/{cardId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid cardId, [FromBody] CardInsertRequest request)
    {
        using var service = new CardService(_appSettings, _logger, GetCurrentUserId());
        return Ok(await service.Update(cardId, request));
    }

    /// <summary>
    /// Atualiza os dados de pagamento de um cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpPut("/v1/card/{cardId}/wallet/{walletId}/payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePayment([FromRoute] Guid cardId, [FromRoute] Guid walletId)
    {
        using var service = new CardService(_appSettings, _logger, GetCurrentUserId());
        return Ok(await service.UpdatePayment(walletId, cardId));
    }

    /// <summary>
    /// Ativa um Cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    [HttpPut("/v1/card/{cardId}/wallet/{walletId}/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Active([FromRoute] Guid walletId, [FromRoute] Guid cardId)
    {
        using var service = new CardService(_appSettings, _logger, GetCurrentUserId());
        return Ok(await service.Active(walletId, cardId));
    }

    /// <summary>
    /// Inativa um Cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    [HttpPut("/v1/card/{cardId}/wallet/{walletId}/inactive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Inactive([FromRoute] Guid walletId, [FromRoute] Guid cardId)
    {
        using var service = new CardService(_appSettings, _logger, GetCurrentUserId());
        return Ok(await service.Inactive(walletId, cardId));
    }

    /// <summary>
    /// Exclui um cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpDelete("/v1/card/{cardId}/wallet/{walletId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid cardId, [FromRoute] Guid walletId)
    {
        using var service = new CardService(_appSettings, _logger, GetCurrentUserId());
        return Ok(await service.Delete(cardId, walletId));
    }
    
    #endregion
}