using FinanceControl.Application.Services.Pix.DTO_s.Request;
using FinanceControl.Application.Services.Pix.Service;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.ControllerBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Controller
{
    public class PixController : BaseController<PixController>
    {
        #region [ Fields ]

        private readonly IPixService _service;

        #endregion

        #region [ Constructor ]

        public PixController(IAppSettings appSettings, IPixService service) : base(appSettings)
        {
            _service = service;
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
            return Ok(await _service.Insert(request));
        }

        /// <summary>
        /// Obtém os dados de um Pix
        /// </summary>
        /// <param name="pixId"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [HttpGet("/v1/pix/{pixId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] Guid pixId)
        {
            return Ok(await _service.GetById(pixId));
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
            return Ok(await _service.GetAll(walletId, search, take, skip));
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
            return Ok(await _service.Update(pixId, request));
        }

        /// <summary>
        /// Exclui um Pix
        /// </summary>
        /// <param name="pixId"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        [HttpDelete("/v1/pix/{pixId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid pixId)
        {
            return Ok(await _service.Delete(pixId));
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
            return Ok(_service.ListPixTypes());
        }

        #endregion

        #endregion
    }
}
