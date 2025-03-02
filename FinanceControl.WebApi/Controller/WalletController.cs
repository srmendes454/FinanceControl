using FinanceControl.Application.Services.Wallet.DTO_s.Request;
using FinanceControl.Application.Services.Wallet.Service;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.ControllerBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Controller;

public class WalletController : BaseController<WalletController>
{
    #region [ Fields ]

    private readonly IWalletService _service;

    #endregion
    #region [ Constructor ]

    public WalletController(IAppSettings appSettings, IWalletService service) : base(appSettings)
    {
        _service = service;
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
        return Ok(await _service.Insert(request));
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
        return Ok(await _service.GetById(walletId));
    }

    /// <summary>
    /// Obtém todas as Carteiras do Usuário
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/wallet")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
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
        return Ok(await _service.Update(walletId, request));
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
        return Ok(await _service.Delete(walletId));
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
        return Ok(await _service.Active(walletId));
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
        return Ok(await _service.Inactive(walletId));
    }

    #region [ Optimize Income ]

    /// <summary>
    /// Serviço para Obter todas as Divisões de Renda por Carteira
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/wallet/{walletId}/optimize-income")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOptimizeIncome([FromRoute] Guid walletId)
    {
        return Ok(await _service.GetAllOptimizeIncome(walletId));
    }

    /// <summary>
    /// Serviço para Obtem uma Divisão da Renda por Carteira
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/wallet/{walletId}/optimize-income/{optimizeIncomeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOptimizeIncomeById([FromRoute] Guid walletId, [FromRoute] Guid optimizeIncomeId)
    {
        return Ok(await _service.GetOptimizeIncomeById(walletId, optimizeIncomeId));
    }

    /// <summary>
    /// Serviço para atualiza uma Divisão de Renda
    /// </summary>
    /// <returns></returns>
    [HttpPut("/v1/wallet/{walletId}/optimize-income/{optimizeIncomeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateOptimizeIncome([FromRoute] Guid walletId, [FromRoute] Guid optimizeIncomeId, [FromBody] OptimizeIncomeRequest request)
    {
        return Ok(await _service.UpdateOptimizeIncome(walletId, optimizeIncomeId, request));
    }

    /// <summary>
    /// Serviço para deletar uma Divisão de Renda
    /// </summary>
    /// <returns></returns>
    [HttpDelete("/v1/wallet/{walletId}/optimize-income/{optimizeIncomeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteOptimizeIncome([FromRoute] Guid walletId, [FromRoute] Guid optimizeIncomeId)
    {
        return Ok(await _service.DeleteOptimizeIncome(walletId, optimizeIncomeId));
    }

    #endregion
    #endregion
}