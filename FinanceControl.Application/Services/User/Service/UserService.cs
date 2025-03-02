using FinanceControl.Application.Extensions.Utils.Cryptography;
using FinanceControl.Application.Extensions.Utils.Email;
using FinanceControl.Application.Services.User.DTO_s;
using FinanceControl.Application.Services.User.Repository;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseService;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.User.Service;

public class UserService : BaseService<UserService>, IUserService
{
    #region [ Fields ]

    private readonly IEmail _email;
    private readonly IUserRepository _repository;

    #endregion

    #region [ Constructor ]
    public UserService(
        IAppSettings appSettings,
        IEmail email,
        IUserRepository repository)
        : base(
            appSettings: appSettings
        )
    {
        _email = email;
        _repository = repository;
    }
    #endregion

    #region [ Messages ]

    private const string PasswordsNotMatch = "As senhas são divergentes";
    private const string PasswordsInvalid = "Senha inválida";
    private const string PasswordEqualsOld = "A senha não pode ser igual a anterior";
    private const string PasswordResetSuccess = "Senha redefinida com sucesso.";
    private const string ReturnPageLogin = "Você será redirecionado para a pagina de Login para entrar com sua nova senha.";
    private const string AccountExists = "Já existe uma conta vinculada a esse email";
    private const string FamilyMembersNotFound = "Nenhum membro familiar foi encontrado";
    private const string FamilyMemberNotFound = "Membro familiar não encontrado";
    private const string SubjectEmail = "Controle Financeiro | Código para redefinição de senha";
    private const string SubjectEmailResetPassword = "Código para redefinição de senha";
    private const string AlertEmailResetPassword = "Caso não tenha solicitado a redefinição de sua senha, favor desconsiderar esse email";
    private const string SubjectEmailWelcome = "Bem-vindo ao seu 'Controle Financeiro' ";
    private const string AlertEmailWelcome = "Sua plataforma para melhor gestão de suas finanças";
    private const string SendEmailFail = "Falha ao enviar o código para seu email, aguarde alguns minutos e tente novamente!";
    private const string SendEmailSucess = "Código enviado com sucesso para o email cadastrado!";

    #endregion

    #region [ Public Methods ]

    /// <summary>
    /// Serviço para criar um Usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> Register(UserInsertRequest request)
    {
        try
        {
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var userExist = await _repository.UserExistByEmail(request.Email);
            if (userExist)
                return ErrorResponse(AccountExists);

            if (request.Password != request.ConfirmPassword)
                return ErrorResponse(PasswordsNotMatch);

            var model = new UserModel(request.Name, request.Email, request.Password.EncryptPassword());

            await _repository.InsertOneAsync(model);

            try
            {
                var template = _email.TemplateWelcome(model.Name, SubjectEmailWelcome, AlertEmailWelcome);
                _email.Send(request.Email, SubjectEmailWelcome, template);
            }
            catch (Exception)
            {
                //ignored
            }

            return SuccessResponse("Usuário", Message.SUCCESSFULLY_ADDED_M.GetEnumDescription());
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

            var user = await _repository.GetByEmail(request.Email);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            if (request.Password.EncryptPassword() != user.Password)
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
    public async Task<ResultValue> Update(UserUpdateRequest request)
    {
        try
        {
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var userId = GetCurrentUserId();
            var user = await _repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            user.Update(request.Name, request.CellPhone, request.Occupation, request.Thumbnail);

            await _repository.Update(userId, user);

            return SuccessResponse("Usuário", Message.SUCCESSFULLY_UPDATED_M.GetEnumDescription());
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
    public async Task<ResultValue> UpdatePassword(UserPasswordRequest request)
    {
        try
        {
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var userId = GetCurrentUserId();
            var user = await _repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            if (user.Password != request.OldPassword.EncryptPassword())
                return ErrorResponse(PasswordsInvalid);

            if (request.NewPassword != request.NewConfirmPassword)
                return ErrorResponse(PasswordsNotMatch);

            if (request.NewPassword.EncryptPassword() == user.Password)
                return ErrorResponse(PasswordEqualsOld);

            user.UpdatePassword(request.NewPassword.EncryptPassword());

            await _repository.UpdatePassword(userId, user);

            return SuccessResponse("Senha", Message.SUCCESSFULLY_UPDATED_M.GetEnumDescription());
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

            var user = await _repository.GetById(userId);
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
            var user = await _repository.GetByEmail(request.Email);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var code = Guid.NewGuid().ToString("N").ToUpper()[..8];
            var template = _email.TemplateResetPassword(user.Name, SubjectEmailResetPassword, AlertEmailResetPassword, code);

            var emailSend = _email.Send(request.Email, SubjectEmail, template);
            if (!emailSend)
                return ErrorResponse(SendEmailFail);

            var resetPassword = user.ResetPassword = new ResetPasswordModel { Code = code };

            await _repository.UpdateCode(user.UserId, resetPassword);
            return SuccessResponse(code, "Controle Financeiro |", SendEmailSucess);
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

            var user = await _repository.GetByEmail(request.Email);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            if (user.Password == request.NewPassword.EncryptPassword())
                return ErrorResponse(PasswordEqualsOld);

            if (request.NewPassword != request.ConfirmNewPassword)
                return ErrorResponse(PasswordsNotMatch);

            user.Password = request.NewPassword.EncryptPassword();
            await _repository.UpdatePassword(user.UserId, user);

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

            var familyMembers = await _repository.GetFamilyMembersByUserId(userId);
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

            var familyMember = await _repository.GetFamilyMemberByUserId(userId, familyId);
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

            var userId = GetCurrentUserId();
            var user = await _repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            user.FamilyMembers ??= new List<FamilyMemberModel>();
            if (request.UserId != Guid.Empty)
            {
                var userFamily = await _repository.GetById(request.UserId);
                if (userFamily != null)
                {
                    user.FamilyMembers.Add(new FamilyMemberModel
                    {
                        UserId = userFamily.UserId,
                        Name = userFamily.Name,
                        Kinship = request.Kinship,
                        Email = userFamily.Email,
                        Active = true,
                        CreationDate = DateTime.UtcNow
                    });
                }
            }
            else
            {
                user.FamilyMembers.Add(new FamilyMemberModel
                {
                    UserId = Guid.Empty,
                    Name = request.Name,
                    Kinship = request.Kinship,
                    Email = request.Email,
                    Active = true,
                    CreationDate = DateTime.UtcNow
                });
            }

            await _repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("Membro Familiar", Message.SUCCESSFULLY_ADDED_M.GetEnumDescription());
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

            var userId = GetCurrentUserId();
            var user = await _repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var familyMember = user.FamilyMembers.FirstOrDefault(fm => fm.FamilyId.Equals(familyId));
            if (familyMember == null)
                return ErrorResponse(FamilyMemberNotFound);

            if (familyMember.UserId != Guid.Empty)
            {
                familyMember.Kinship = request.Kinship;
                familyMember.UpdateDate = DateTime.UtcNow;
            }
            else
            {
                familyMember.Name = request.Name;
                familyMember.Email = request.Email;
                familyMember.Kinship = request.Kinship;
                familyMember.UpdateDate = DateTime.UtcNow;
            }

            await _repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("Membro Familiar", Message.SUCCESSFULLY_UPDATED_M.GetEnumDescription());
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

            var userId = GetCurrentUserId();
            var user = await _repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var familyMember = user.FamilyMembers?.FirstOrDefault(f => f.FamilyId.Equals(familyId));
            if (familyMember == null)
                return ErrorResponse(FamilyMemberNotFound);

            familyMember.Active = familyMember.Active == false;

            await _repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("Membro Familiar", Message.SUCCESSFULLY_UPDATED_M.GetEnumDescription());
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

            var userId = GetCurrentUserId();
            var user = await _repository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var familyMember = user.FamilyMembers?.FirstOrDefault(f => f.FamilyId.Equals(familyId));
            if (familyMember == null)
                return ErrorResponse(FamilyMemberNotFound);

            user.FamilyMembers.Remove(familyMember);
            await _repository.UpdateFamilyMembers(userId, user);

            return SuccessResponse("Membro Familiar", Message.SUCCESSFULLY_DELETED_M.GetEnumDescription());
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
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.PrimarySid, userId.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email)
            ]),
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