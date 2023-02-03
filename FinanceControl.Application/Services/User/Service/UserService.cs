using FinanceControl.Extensions.AppSettings;
using FinanceControl.Extensions.Enum;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.BaseService;
using FinanceControl.Application.Services.User.DTO_s;
using FinanceControl.Application.Services.User.Model;
using FinanceControl.Application.Services.User.Repository;
using ILogger = Serilog.ILogger;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Wallet.DTO_s.Response;

namespace FinanceControl.Application.Services.User.Service;

public class UserService : BaseService
{
    #region [ Constructor ]
    public UserService(IAppSettings appSettings, ILogger logger,
        Guid currentUserId) : base(logger: logger, appSettings: appSettings,
        currentUserId: currentUserId)
    {
    }
    #endregion

    #region [ Messages ]

    private const string PasswordsNotMatch = "PASSWORDS_DO_NOT_MATCH";
    private const string PasswordsInvalid = "PASSWORD_INVALID";
    private const string PasswordSamePrevious = "PASSWORD_SAME_AS_PREVIOUS";

    #endregion

    #region [ Public Methods ]

    /// <summary>
    /// Serviço para criar um Usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> RegisterUser(UserInsertRequest request)
    {
        try
        {
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            if (request.Password != request.ConfirmPassword)
                return ErrorResponse(PasswordsNotMatch);

            var model = new UserModel
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                CreationDate = DateTime.Now,
                Active = true
            };

            using var repository = new UserRepository(_appSettings.GetMongoDb(), _logger);

            await repository.InsertOneAsync(model);

            return SuccessResponse("USER", Message.SUCCESSFULLY_ADDED.ToString());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para autenticar um Usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> Login(UserLoginRequest request)
    {
        try
        {
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var user = await repository.GetByEmail(request.Email);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.ToString());

            var password = user.Password;
            if (request.Password != password)
                return ErrorResponse(PasswordsInvalid);

            var token = GenerateToken(user.UserId, user.Email, user.Name);
            return SuccessResponse($"Bearer {token}");
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para atualizar um Usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> UpdateUser(UserUpdateRequest request)
    {
        try
        {
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.ToString());

            user.Thumbnail = request.Thumbnail ?? user.Thumbnail;
            user.CellPhone = request.CellPhone ?? user.CellPhone;
            user.Occupation = request.Occupation ?? user.Occupation;
            user.Name = request.Name ?? user.Name;

            await repository.Update(userId, user);

            return SuccessResponse("USER", Message.SUCCESSFULLY_UPDATED.ToString());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para atualizar a senha do Usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> UpdatePasswordUser(UserPasswordRequest request)
    {
        try
        {
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.ToString());

            if (user.Password != request.OldPassword)
                return ErrorResponse(PasswordsInvalid);

            if (request.NewPassword != request.NewConfirmPassword)
                return ErrorResponse(PasswordsNotMatch);

            if (request.NewPassword == user.Password)
                return ErrorResponse(PasswordSamePrevious);

            user.Password = request.NewPassword;

            await repository.UpdatePassword(userId, user);

            return SuccessResponse("PASSWORD", Message.SUCCESSFULLY_UPDATED.ToString());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Obter os dados do Usuário
    /// </summary>
    /// <returns></returns>
    public async Task<ResultValue> GetById()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.ToString());

            var result = _mapper.Map<UserResponse>(user);

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }
    #endregion

    #region [ Private Methods ]

    /// <summary>
    /// Serviço para gerar o TOKEN JWT
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="email"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private string GenerateToken(Guid userId, string email, string name)
    {
        var key = Encoding.UTF8.GetBytes(_appSettings.GetConfiguration()["Tokens:Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.PrimarySid, userId.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email)
            }),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var result = tokenHandler.WriteToken(token);

        return result;
    }
    #endregion
}