using FinanceControl.Extensions.AppSettings;
using FinanceControl.Extensions.Enum;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
    private const string FamilyMembersNotFound = "FAMILY_MEMBERS_NOT_FOUND";
    private const string FamilyMemberNotFound = "FAMILY_MEMBER_NOT_FOUND";

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
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

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
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

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
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.ToString());

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

            return SuccessResponse("FAMILY_MEMBER", Message.SUCCESSFULLY_ADDED.ToString());
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
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.ToString());

            var familyMember = user.FamilyMembers?.FirstOrDefault(f => f.FamilyId.Equals(familyId));
            if (familyMember == null)
                return ErrorResponse(FamilyMemberNotFound);

            familyMember.Name = request.Name;
            familyMember.Email = request.Email;
            familyMember.Kinship = request.Kinship;
            familyMember.UpdateDate = DateTime.UtcNow;

            await repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("FAMILY_MEMBER", Message.SUCCESSFULLY_UPDATED.ToString());
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
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.ToString());

            var familyMember = user.FamilyMembers?.FirstOrDefault(f => f.FamilyId.Equals(familyId));
            if (familyMember == null)
                return ErrorResponse(FamilyMemberNotFound);

            familyMember.Active = familyMember.Active == false;

            await repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("FAMILY_MEMBER", Message.SUCCESSFULLY_UPDATED.ToString());
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
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var userId = GetCurrentUserId();
            var user = await repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.ToString());

            var familyMember = user.FamilyMembers?.FirstOrDefault(f => f.FamilyId.Equals(familyId));
            if (familyMember == null)
                return ErrorResponse(FamilyMemberNotFound);

            user.FamilyMembers.Remove(familyMember);
            await repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("FAMILY_MEMBER", Message.SUCCESSFULLY_DELETED.ToString());
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