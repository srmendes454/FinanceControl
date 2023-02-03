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

namespace FinanceControl.Controller;

public class UserController : BaseController
{
    #region [ Fields ]

    private readonly IRequestContainer _request;

    #endregion
    #region [ Contructor ]
    public UserController(IAppSettings appSettings, IRequestContainer request) : base(appSettings)
    {
        _logger = appSettings.GetLogger().ForContext<UserController>();
        _request = request;
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
        using var service = new UserService(_appSettings, _logger, Guid.Empty);
        return Ok(await service.RegisterUser(request));
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
        using var service = new UserService(_appSettings, _logger, Guid.Empty);
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
        using var service = new UserService(_appSettings, _logger, _request.UserId);
        return Ok(await service.UpdateUser(request));
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
        using var service = new UserService(_appSettings, _logger, _request.UserId);
        return Ok(await service.UpdatePasswordUser(request));
    }

    /// <summary>
    /// Obtém os dados do Usuário
    /// </summary>
    /// <returns></returns>
    [HttpGet("/v1/user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById()
    {
        using var service = new UserService(_appSettings, _logger, _request.UserId);
        return Ok(await service.GetById());
    }
    #endregion
}