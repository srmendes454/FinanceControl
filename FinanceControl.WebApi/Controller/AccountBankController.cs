using FinanceControl.Application.Services.AccountBank.DTO_s.Request;
using FinanceControl.Application.Services.AccountBank.Service;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.ControllerBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Controller
{
    public class AccountBank : BaseController<AccountBank>
    {
        #region [ Fields ]

        private readonly IAccountBankService _service;

        #endregion

        #region [ Constructor ]

        public AccountBank(IAppSettings appSettings, IAccountBankService service) : base(appSettings)
        {
            _service = service;
        }

        #endregion

        #region [ Public Routes ]

        /// <summary>
        /// Insere uma Conta Bancária
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/v1/account-bank")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> InsertCard([FromBody] AccountBankInsertRequest request)
        {
            return Ok(await _service.Insert(request));
        }

        /// <summary>
        /// Obtém os dados de uma Conta Bancária
        /// </summary>
        /// <param name="accountBankId"></param>
        /// <returns></returns>
        [HttpGet("/v1/account-bank/{accountBankId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] Guid accountBankId)
        {
            return Ok(await _service.GetById(accountBankId));
        }

        /// <summary>
        /// Obtém todos as Contas Bancárias por Carteira
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        [HttpGet("/v1/account-bank/wallet/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromRoute] Guid walletId, [FromQuery] string search = null, [FromQuery] int take = 20, [FromQuery] int skip = 1)
        {
            return Ok(await _service.GetAll(walletId, search, take, skip));
        }

        /// <summary>
        /// Atualiza os dados de uma Conta Bancária
        /// </summary>
        /// <param name="accountBankId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("/v1/account-bank/{accountBankId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] Guid accountBankId, [FromBody] AccountBankInsertRequest request)
        {
            return Ok(await _service.Update(accountBankId, request));
        }

        /// <summary>
        /// Exclui uma Conta Bancária
        /// </summary>
        /// <param name="accountBankId"></param>
        /// <returns></returns>
        [HttpDelete("/v1/account-bank/{accountBankId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid accountBankId)
        {
            return Ok(await _service.Delete(accountBankId));
        }

        #region [ List Enuns ]

        /// <summary>
        /// Lista todos os tipos de Contas Bancárias
        /// </summary>
        /// <returns></returns>
        [HttpGet("/v1/account-bank/type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ListAccountBankTypes()
        {
            return Ok(_service.ListAccountBankTypes());
        }

        #endregion

        #endregion
    }
}
