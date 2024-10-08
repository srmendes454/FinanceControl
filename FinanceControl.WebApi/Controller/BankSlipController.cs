using FinanceControl.Application.Extensions.ControllerBase;
using FinanceControl.Application.Extensions.RequestContainer;
using FinanceControl.Application.Services.BankSlip.DTO_s.Request;
using FinanceControl.Application.Services.BankSlip.Service;
using FinanceControl.Extensions.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Controller
{
    public class BankSlipController : BaseController
    {
        #region [ Fields ]

        private readonly IRequestContainer _request;

        #endregion

        #region [ Constructor ]

        public BankSlipController(IAppSettings appSettings, IRequestContainer request) : base(appSettings)
        {
            _logger = appSettings.GetLogger().ForContext<BankSlipController>();
            _request = request;
        }

        #endregion

        #region [ Public Routes ]

        /// <summary>
        /// Insere um Boleto
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/v1/bank-slip")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> InsertCard([FromBody] BankSlipInsertRequest request)
        {
            using var service = new BankSlipService(_appSettings, _logger, _request.UserId);
            return Ok(await service.Insert(request));
        }

        /// <summary>
        /// Obtém os dados de um Boleto
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [HttpGet("/v1/bank-slip/{bankSlipId}/wallet/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] Guid bankSlipId, [FromRoute] Guid walletId)
        {
            using var service = new BankSlipService(_appSettings, _logger, _request.UserId);
            return Ok(await service.GetById(walletId, bankSlipId));
        }

        /// <summary>
        /// Obtém todos os Boletos
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        [HttpGet("/v1/bank-slip/wallet/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromRoute] Guid walletId, [FromQuery] string search = null, [FromQuery] int take = 20, [FromQuery] int skip = 1)
        {
            using var service = new BankSlipService(_appSettings, _logger, _request.UserId);
            return Ok(await service.GetAll(walletId, search, take, skip));
        }

        /// <summary>
        /// Atualiza os dados de um Boleto
        /// </summary>
        /// <param name="bankSlipId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("/v1/bank-slip/{bankSlipId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] Guid bankSlipId, [FromBody] BankSlipUpdateRequest request)
        {
            using var service = new BankSlipService(_appSettings, _logger, _request.UserId);
            return Ok(await service.Update(bankSlipId, request));
        }

        /// <summary>
        /// Exclui um Boleto
        /// </summary>
        /// <param name="bankSlipId"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [HttpDelete("/v1/bank-slip/{bankSlipId}/wallet/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid bankSlipId, [FromRoute] Guid walletId)
        {
            using var service = new BankSlipService(_appSettings, _logger, _request.UserId);
            return Ok(await service.Delete(bankSlipId, walletId));
        }

        #endregion
    }
}
