using FinanceControl.Application.Extensions.ControllerBase;
using FinanceControl.Application.Extensions.RequestContainer;
using FinanceControl.Application.Services.Cards.Service;
using FinanceControl.Application.Services.Pix.DTO_s.Request;
using FinanceControl.Application.Services.Pix.Service;
using FinanceControl.Extensions.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Controller
{
    public class PixController : BaseController
    {
        #region [ Fields ]

        private readonly IRequestContainer _request;

        #endregion

        #region [ Constructor ]

        public PixController(IAppSettings appSettings, IRequestContainer request) : base(appSettings)
        {
            _logger = appSettings.GetLogger().ForContext<PixController>();
            _request = request;
        }

        #endregion

        #region [ Public Routes ]

        /// <summary>
        /// Insere um Pix
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/v1/pix")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> InsertCard([FromBody] PixInsertRequest request)
        {
            using var service = new PixService(_appSettings, _logger, _request.UserId);
            return Ok(await service.Insert(request));
        }

        /// <summary>
        /// Obtém os dados de um Pix
        /// </summary>
        /// <param name="pixId"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [HttpGet("/v1/pix/{pixId}/wallet/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] Guid pixId, [FromRoute] Guid walletId)
        {
            using var service = new PixService(_appSettings, _logger, _request.UserId);
            return Ok(await service.GetById(walletId, pixId));
        }

        /// <summary>
        /// Obtém todos os Pixs
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        [HttpGet("/v1/pix/wallet/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromRoute] Guid walletId, [FromQuery] string search = null, [FromQuery] int take = 20, [FromQuery] int skip = 1)
        {
            using var service = new PixService(_appSettings, _logger, _request.UserId);
            return Ok(await service.GetAll(walletId, search, take, skip));
        }

        /// <summary>
        /// Atualiza os dados de um Pix
        /// </summary>
        /// <param name="pixId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("/v1/pix/{pixId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] Guid pixId, [FromBody] PixInsertRequest request)
        {
            using var service = new PixService(_appSettings, _logger, _request.UserId);
            return Ok(await service.Update(pixId, request));
        }

        /// <summary>
        /// Exclui um Pix
        /// </summary>
        /// <param name="pixId"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [HttpDelete("/v1/pix/{pixId}/wallet/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid pixId, [FromRoute] Guid walletId)
        {
            using var service = new PixService(_appSettings, _logger, _request.UserId);
            return Ok(await service.Delete(pixId, walletId));
        }

        #region [ List Enuns ]

        /// <summary>
        /// Lista todos os tipos de Chaves Pix
        /// </summary>
        /// <returns></returns>
        [HttpGet("/v1/pix/type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ListPixTypes()
        {
            using var service = new PixService(_appSettings, _logger, _request.UserId);
            return Ok(service.ListPixTypes());
        }

        #endregion

        #endregion
    }
}
