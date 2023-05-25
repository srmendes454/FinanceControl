using FinanceControl.Application.Extensions.BaseService;
using FinanceControl.Application.Extensions.Utils.Email;
using FinanceControl.Application.Services.User.DTO_s;
using FinanceControl.Application.Services.User.Model;
using FinanceControl.Application.Services.User.Repository;
using FinanceControl.Extensions.AppSettings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.Enum;
using Microsoft.OpenApi.Extensions;
using ILogger = Serilog.ILogger;

namespace FinanceControl.Application.Services.User.Service;

public class UserService : BaseService
{
    #region [ Fields ]

    private readonly IEmail _email;

    #endregion

    #region [ Constructor ]
    public UserService(IAppSettings appSettings, ILogger logger,
        Guid currentUserId, IEmail email) : base(logger: logger, appSettings: appSettings,
        currentUserId: currentUserId)
    {
        _email = email;
    }
    #endregion

    #region [ Messages ]

    private const string PasswordsNotMatch = "As senhas são divergentes";
    private const string PasswordsInvalid = "Senha inválida";
    private const string PasswordEqualsOld = "A senha não pode ser igual a anterior";
    private const string PasswordResetSuccess = "Senha redefinida com sucesso.";
    private const string ReturnPageLogin = "Retorne a pagina de Login para entrar com sua nova senha.";
    private const string CodeVerification = "Seu código de verificação é";
    private const string FamilyMembersNotFound = "Nenhum membro familiar foi encontrado";
    private const string FamilyMemberNotFound = "Membro familiar não encontrado";
    private const string SubjectEmail = "Controle Financeiro | Código para redefinição de senha";

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
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

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

            return SuccessResponse("Usuário", Message.SUCCESSFULLY_ADDED.GetEnumDescription());
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
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var user = await repository.GetByEmail(request.Email);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

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
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            user.Thumbnail = request.Thumbnail ?? user.Thumbnail;
            user.CellPhone = request.CellPhone ?? user.CellPhone;
            user.Occupation = request.Occupation ?? user.Occupation;
            user.Name = request.Name ?? user.Name;

            await repository.Update(userId, user);

            return SuccessResponse("Usuário", Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
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
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            if (user.Password != request.OldPassword)
                return ErrorResponse(PasswordsInvalid);

            if (request.NewPassword != request.NewConfirmPassword)
                return ErrorResponse(PasswordsNotMatch);

            if (request.NewPassword == user.Password)
                return ErrorResponse(PasswordEqualsOld);

            user.Password = request.NewPassword;

            await repository.UpdatePassword(userId, user);

            return SuccessResponse("Senha", Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
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
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var result = _mapper.Map<UserResponse>(user);

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para validar usuário e envio de código para resetar senhar
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> SendEmailWithCodeToResetPassword(UserSendEmailRequest request)
    {
        try
        {
            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
            var user = await repository.GetByEmail(request.Email);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var code = Guid.NewGuid().ToString("N").ToUpper()[..8];
            var message = $"{CodeVerification}: {code}";
            var emailSend = _email.Send(request.Email, SubjectEmail, message);
            if (!emailSend)
                return ErrorResponse(Message.SEND_EMAIL_FAIL.GetEnumDescription());
            
            var resetPassword = user.ResetPassword = new ResetPasswordModel {Code = code};

            await repository.UpdateCode(user.UserId, resetPassword);
            return SuccessResponse("Controle Financeiro |", Message.SEND_EMAIL_SUCCESS.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para validar usuário com código enviado e resetar senhar
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> ResetPassword(UserResetPasswordRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
            var user = await repository.GetByEmail(request.Email);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            if(user.ResetPassword?.Code != request.Code)
                return ErrorResponse(Message.CODE_INVALID.GetEnumDescription());

            if (user.Password == request.NewPassword)
                return ErrorResponse(PasswordEqualsOld);

            if (request.NewPassword != request.NewConfirmPassword)
                return ErrorResponse(PasswordsNotMatch);

            user.Password = request.NewPassword;
            await repository.UpdatePassword(user.UserId, user);

            return SuccessResponse(PasswordResetSuccess, ReturnPageLogin);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    #region [ Family Members ]

    /// <summary>
    /// Serviço para Obter os Membros Familiares do Usuário
    /// </summary>
    /// <returns></returns>
    public async Task<ResultValue> GetFamilyMembersByUserId()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
            var familyMembers = await repository.GetFamilyMembersByUserId(userId);
            if (familyMembers == null || familyMembers.Count <= 0)
                return SuccessResponse(FamilyMembersNotFound);

            var result = _mapper.Map<List<FamilyMembersResponse>>(familyMembers);

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Obter os Membros Familiares do Usuário
    /// </summary>
    /// <param name="familyId"></param>
    /// <returns></returns>
    public async Task<ResultValue> GetFamilyMemberByUserId(Guid familyId)
    {
        try
        {
            if (familyId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
            var familyMember = await repository.GetFamilyMemberByUserId(userId, familyId);
            if (familyMember == null)
                return ErrorResponse(FamilyMemberNotFound);

            var result = _mapper.Map<FamilyMembersResponse>(familyMember);

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para inserir um Membro Familiar do Usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> InsertFamilyMember(FamilyMemberRequest request)
    {
        try
        {
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var familyMember = new FamilyMemberModel
            {
                Name = request.Name,
                Kinship = request.Kinship,
                Email = request.Email,
                Active = true,
                CreationDate = DateTime.UtcNow
            };
            user.FamilyMembers.Add(familyMember);

            await repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("Membro Familiar", Message.SUCCESSFULLY_ADDED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para atualizar um Membro Familiar do Usuário
    /// </summary>
    /// <param name="familyId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> UpdateFamilyMember(Guid familyId, FamilyMemberRequest request)
    {
        try
        {
            if (familyId == Guid.Empty || request == null)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var familyMember = user.FamilyMembers?.FirstOrDefault(f => f.FamilyId.Equals(familyId));
            if (familyMember == null)
                return ErrorResponse(FamilyMemberNotFound);

            familyMember.Name = request.Name;
            familyMember.Email = request.Email;
            familyMember.Kinship = request.Kinship;
            familyMember.UpdateDate = DateTime.UtcNow;

            await repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("Membro Familiar", Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para ativar ou inativar um Membro Familiar do Usuário
    /// </summary>
    /// <param name="familyId"></param>
    /// <param name="active"></param>
    /// <returns></returns>
    public async Task<ResultValue> ActiveInactiveFamilyMember(Guid familyId)
    {
        try
        {
            if (familyId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var familyMember = user.FamilyMembers?.FirstOrDefault(f => f.FamilyId.Equals(familyId));
            if (familyMember == null)
                return ErrorResponse(FamilyMemberNotFound);

            familyMember.Active = familyMember.Active == false;

            await repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("Membro Familiar", Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para apagar um Membro Familiar do Usuário
    /// </summary>
    /// <param name="familyId"></param>
    /// <returns></returns>
    public async Task<ResultValue> DeleteFamilyMember(Guid familyId)
    {
        try
        {
            if (familyId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var familyMember = user.FamilyMembers?.FirstOrDefault(f => f.FamilyId.Equals(familyId));
            if (familyMember == null)
                return ErrorResponse(FamilyMemberNotFound);

            user.FamilyMembers.Remove(familyMember);
            await repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("Membro Familiar", Message.SUCCESSFULLY_DELETED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }
    #endregion
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