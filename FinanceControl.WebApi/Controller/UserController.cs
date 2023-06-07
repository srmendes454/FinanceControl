using FinanceControl.Application.Extensions.ControllerBase;
using FinanceControl.Application.Extensions.RequestContainer;
using FinanceControl.Application.Services.User.DTO_s;
using FinanceControl.Application.Services.User.Service;
using FinanceControl.Extensions.AppSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.Utils.Email;

namespace FinanceControl.Controller;

public class UserController : BaseController
{
    #region [ Fields ]

    private readonly IRequestContainer _request;
    private readonly IEmail _email;

    #endregion
    #region [ Contructor ]
    public UserController(IAppSettings appSettings, IRequestContainer request, IEmail email) : base(appSettings)
    {
        _logger = appSettings.GetLogger().ForContext<UserController>();
        _request = request;
        _email = email;
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
        using var service = new UserService(_appSettings, _logger, Guid.Empty, _email);
        return Ok(await service.Register(request));
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
        using var service = new UserService(_appSettings, _logger, Guid.Empty, _email);
        return Ok(await service.Login(request));
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
        using var service = new UserService(_appSettings, _logger, _request.UserId, _email);
        return Ok(await service.Update(request));
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
        using var service = new UserService(_appSettings, _logger, _request.UserId, _email);
        return Ok(await service.UpdatePassword(request));
    }

    /// <summary>
    /// Obtém os dados do Usuário
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById()
    {
        using var service = new UserService(_appSettings, _logger, _request.UserId, _email);
        return Ok(await service.GetById());
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
        using var service = new UserService(_appSettings, _logger, Guid.Empty, _email);
        return Ok(await service.SendEmailWithCodeToResetPassword(request));
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
        using var service = new UserService(_appSettings, _logger, Guid.Empty, _email);
        return Ok(await service.ResetPassword(request));
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
        using var service = new UserService(_appSettings, _logger, _request.UserId, _email);
        return Ok(await service.GetFamilyMembersByUserId());
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
        using var service = new UserService(_appSettings, _logger, _request.UserId, _email);
        return Ok(await service.GetFamilyMemberByUserId(familyId));
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
        using var service = new UserService(_appSettings, _logger, _request.UserId, _email);
        return Ok(await service.InsertFamilyMember(request));
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
        using var service = new UserService(_appSettings, _logger, _request.UserId, _email);
        return Ok(await service.UpdateFamilyMember(familyId, request));
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
        using var service = new UserService(_appSettings, _logger, _request.UserId, _email);
        return Ok(await service.ActiveInactiveFamilyMember(familyId));
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
        using var service = new UserService(_appSettings, _logger, _request.UserId, _email);
        return Ok(await service.DeleteFamilyMember(familyId));
    }
    #endregion
    #endregion
}