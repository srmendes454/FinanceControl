using FinanceControl.Application.Services.BankSlip.DTO_s.Request;
using FinanceControl.Application.Services.BankSlip.Service;
using FinanceControl.Application.Services.Cards.Service;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.ControllerBase;
using FinanceControl.Infra.RequestContainer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Controller
{
    public class BankSlipController : BaseController<BankSlipController>
    {
        #region [ Fields ]

        private readonly IBankSlipService _service;

        #endregion

        #region [ Constructor ]

        public BankSlipController(IAppSettings appSettings, IBankSlipService service) : base(appSettings)
        {
            _service = service;
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
            return Ok(await _service.Insert(request));
        }

        /// <summary>
        /// Obtém os dados de um Boleto
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [HttpGet("/v1/bank-slip/{bankSlipId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] Guid bankSlipId)
        {
            return Ok(await _service.GetById(bankSlipId));
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
            return Ok(await _service.GetAll(walletId, search, take, skip));
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
            return Ok(await _service.Update(bankSlipId, request));
        }

        /// <summary>
        /// Exclui um Boleto
        /// </summary>
        /// <param name="bankSlipId"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [HttpDelete("/v1/bank-slip/{bankSlipId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid bankSlipId)
        {
            return Ok(await _service.Delete(bankSlipId));
        }

        #endregion
    }
}
