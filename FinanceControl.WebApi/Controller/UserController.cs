using FinanceControl.Application.Services.User.DTO_s;
using FinanceControl.Application.Services.User.Service;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.ControllerBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Controller;

public class UserController : BaseController<UserController>
{
    #region [ Fields ]

    private readonly IUserService _service;

    #endregion
    #region [ Contructor ]
    public UserController(IAppSettings appSettings, IUserService service) : base(appSettings)
    {
        _service = service;
    }
    #endregion

    #region [ Public Routes ]

    /// <summary>
    /// Registra um novo Usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/v1/user/register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterUser([FromBody] UserInsertRequest request)
    {
        return Ok(await _service.Register(request));
    }

    /// <summary>
    /// Entrar na aplicação
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/v1/user/login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        return Ok(await _service.Login(request));
    }

    /// <summary>
    /// Atualiza os dados do usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("/v1/user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequest request)
    {
        return Ok(await _service.Update(request));
    }

    /// <summary>
    /// Atualiza a senha do usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("/v1/user/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePasswordUser([FromBody] UserPasswordRequest request)
    {
        return Ok(await _service.UpdatePassword(request));
    }

    /// <summary>
    /// Obtém os dados do Usuário
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById()
    {
        return Ok(await _service.GetById());
    }

    /// <summary>
    /// Serviço para validar usuário e envio de código para resetar senhar
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/v1/user/send-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendEmailWithCodeToResetPassword([FromBody] UserSendEmailRequest request)
    {
        return Ok(await _service.SendEmailWithCodeToResetPassword(request));
    }

    /// <summary>
    /// Serviço para validar usuário com código enviado e resetar senhar
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("/v1/user/reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordRequest request)
    {
        return Ok(await _service.ResetPassword(request));
    }

    #region [ Family Members ]

    /// <summary>
    /// Obtém os membros familiares do Usuário
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/user/family-members")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFamilyMembersByUserId()
    {
        return Ok(await _service.GetFamilyMembersByUserId());
    }

    /// <summary>
    /// Obtém o membro familiar do Usuário
    /// </summary>
    /// <param name="familyId"></param>
    /// <returns></returns>
    [HttpGet("/v1/user/family-members/{familyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFamilyMemberByUserId([FromRoute] Guid familyId)
    {
        return Ok(await _service.GetFamilyMemberByUserId(familyId));
    }

    /// <summary>
    /// Insere um membro familiar do Usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("/v1/user/family-members")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertFamilyMember([FromBody] FamilyMemberRequest request)
    {
        return Ok(await _service.InsertFamilyMember(request));
    }

    /// <summary>
    /// Atualiza um membro familiar do Usuário
    /// </summary>
    /// <param name="familyId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("/v1/user/family-members/{familyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFamilyMember([FromRoute] Guid familyId, [FromBody] FamilyMemberRequest request)
    {
        return Ok(await _service.UpdateFamilyMember(familyId, request));
    }

    /// <summary>
    /// Inativa ou Ativa um membro familiar do Usuário
    /// </summary>
    /// <param name="familyId"></param>
    /// <returns></returns>
    [HttpPut("/v1/user/family-members/{familyId}/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ActiveInactiveFamilyMember([FromRoute] Guid familyId)
    {
        return Ok(await _service.ActiveInactiveFamilyMember(familyId));
    }

    /// <summary>
    /// Apaga um membro familiar do Usuário
    /// </summary>
    /// <param name="familyId"></param>
    /// <returns></returns>
    [HttpPut("/v1/user/family-members/{familyId}/delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteFamilyMember([FromRoute] Guid familyId)
    {
        return Ok(await _service.DeleteFamilyMember(familyId));
    }
    #endregion
    #endregion
}