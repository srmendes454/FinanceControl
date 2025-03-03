using FinanceControl.Application.Services.Cards.DTO_s;
using FinanceControl.Application.Services.Division.DTO_s.Request;
using FinanceControl.Application.Services.Division.Service;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.ControllerBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceControl.Controller
{
    public class DivisionController : BaseController<DivisionController>
    {
        #region [ Fields ]

        private readonly IDivisionService _service;

        #endregion

        #region [ Contructor ]
        public DivisionController(IAppSettings appSettings, IDivisionService service) : base(appSettings)
        {
            _service = service;
        }
        #endregion

        #region [ Public Routes ]

        /// <summary>
        /// Insere uma Repartição
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/v1/division")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Insert([FromBody] DivisionInsertRequest request)
        {
            return Ok(await _service.Insert(request));
        }

        /// <summary>
        /// Obtém uma Repartição
        /// </summary>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        [HttpGet("/v1/division/{divisionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] Guid divisionId)
        {
            return Ok(await _service.GetById(divisionId));
        }

        /// <summary>
        /// Obtém todos as Divisões
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        [HttpGet("/v1/division/wallet/{walletId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromRoute] Guid walletId, [FromQuery] int year, [FromQuery] int month, [FromQuery] string search = null, [FromQuery] int take = 20, [FromQuery] int skip = 1)
        {
            return Ok(await _service.GetAll(walletId, search, year, month, take, skip));
        }

        /// <summary>
        /// Atualiza os dados de uma Repartição
        /// </summary>
        /// <param name="divisionId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("/v1/division/{divisionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] Guid divisionId, [FromBody] DivisionInsertRequest request)
        {
            return Ok(await _service.Update(divisionId, request));
        }

        /// <summary>
        /// Exclui uma Repartição
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        [HttpDelete("/v1/division/{divisionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid divisionId)
        {
            return Ok(await _service.Delete(divisionId));
        }

        #region [ Limits ]

        /// <summary>
        /// Inserir Limite para uma repartição
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/v1/division/{divisionId}/limit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveLimit([FromRoute] Guid divisionId, [FromBody] List<LimitInsertRequest> request)
        {
            return Ok(await _service.SaveLimit(divisionId, request));
        }

        #endregion

        #endregion
    }
}
