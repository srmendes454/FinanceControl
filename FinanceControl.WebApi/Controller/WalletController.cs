using FinanceControl.Extensions.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.ControllerBase;
using FinanceControl.Application.Extensions.RequestContainer;
using FinanceControl.Application.Services.Wallet.DTO_s.Request;
using FinanceControl.Application.Services.Wallet.Service;

namespace FinanceControl.Controller;

public class WalletController : BaseController
{
    #region [ Fields ]

    private readonly IRequestContainer _request;

    #endregion
    #region [ Constructor ]

    public WalletController(IAppSettings appSettings, IRequestContainer request) : base(appSettings)
    {
        _logger = appSettings.GetLogger().ForContext<WalletController>();
        _request = request;
    }

    #endregion

    #region [ Public Routes ]

    /// <summary>
    /// Insere uma Carteira
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/v1/wallet")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertCard([FromBody] WalletInsertRequest request)
    {
        using var service = new WalletService(_appSettings, _logger, _request.UserId);
        return Ok(await service.Insert(request));
    }

    /// <summary>
    /// Obtém uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpGet("/v1/wallet/{walletId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid walletId)
    {
        using var service = new WalletService(_appSettings, _logger, _request.UserId);
        return Ok(await service.GetById(walletId));
    }

    /// <summary>
    /// Obtém todas as Carteiras do Usuário
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/wallet")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        using var service = new WalletService(_appSettings, _logger, _request.UserId);
        return Ok(await service.GetAll());
    }

    /// <summary>
    /// Atualiza os dados de uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("/v1/wallet/{walletId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid walletId, [FromBody] WalletInsertRequest request)
    {
        using var service = new WalletService(_appSettings, _logger, _request.UserId);
        return Ok(await service.Update(walletId, request));
    }

    /// <summary>
    /// Exclui uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpDelete("/v1/wallet/{walletId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid walletId)
    {
        using var service = new WalletService(_appSettings, _logger, _request.UserId);
        return Ok(await service.Delete(walletId));
    }

    /// <summary>
    /// Ativa uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpPut("/v1/wallet/{walletId}/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Active([FromRoute] Guid walletId)
    {
        using var service = new WalletService(_appSettings, _logger, _request.UserId);
        return Ok(await service.Active(walletId));
    }

    /// <summary>
    /// Inativa uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpPut("/v1/wallet/{walletId}/inactive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Inactive([FromRoute] Guid walletId)
    {
        using var service = new WalletService(_appSettings, _logger, _request.UserId);
        return Ok(await service.Inactive(walletId));
    }
    #endregion
}