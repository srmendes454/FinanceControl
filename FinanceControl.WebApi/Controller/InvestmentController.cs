using FinanceControl.Application.Services.AccountBank.DTO_s.Request;
using FinanceControl.Application.Services.AccountBank.Service;
using FinanceControl.Application.Services.Investment.DTO_s.Request;
using FinanceControl.Application.Services.Investment.Service;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.ControllerBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Controller
{
    public class InvestmentController : BaseController<InvestmentController>
    {
        #region [ Fields ]

        private readonly IInvestmentService _service;

        #endregion

        #region [ Constructor ]

        public InvestmentController(IAppSettings appSettings, IInvestmentService service) : base(appSettings)
        {
            _service = service;
        }

        #endregion

        #region [ Public Routes ]

        /// <summary>
        /// Insere um Investimento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/v1/investment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> InsertCard([FromBody] InvestmentInsertRequest request)
        {
            return Ok(await _service.Insert(request));
        }

        /// <summary>
        /// Obtém os dados de um Investimento
        /// </summary>
        /// <param name="investmentId"></param>
        /// <returns></returns>
        [HttpGet("/v1/investment/{investmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] Guid investmentId)
        {
            return Ok(await _service.GetById(investmentId));
        }

        /// <summary>
        /// Obtém todos os Investimentos
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        [HttpGet("/v1/investment/wallet/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromRoute] Guid walletId, [FromQuery] string search = null, [FromQuery] int take = 20, [FromQuery] int skip = 1)
        {
            return Ok(await _service.GetAll(walletId, search, take, skip));
        }

        /// <summary>
        /// Atualiza os dados de um Investimento
        /// </summary>
        /// <param name="investmentId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("/v1/investment/{investmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] Guid investmentId, [FromBody] InvestmentInsertRequest request)
        {
            return Ok(await _service.Update(investmentId, request));
        }

        /// <summary>
        /// Exclui um Investimento
        /// </summary>
        /// <param name="investmentId"></param>
        /// <returns></returns>
        [HttpDelete("/v1/investment/{investmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid investmentId)
        {
            return Ok(await _service.Delete(investmentId));
        }

        #region [ List Enuns ]

        /// <summary>
        /// Lista todos os tipos de Investimentos
        /// </summary>
        /// <returns></returns>
        [HttpGet("/v1/investment/type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ListInvestmentTypes()
        {
            return Ok(_service.ListInvestmentTypes());
        }

        #endregion

        #endregion
    }
}
