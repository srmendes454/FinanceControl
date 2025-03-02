using FinanceControl.Application.Services.User.DTO_s;
using FinanceControl.Infra.BaseService;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.User.Service
{
    public interface IUserService
    {
        Task<ResultValue> Register(UserInsertRequest request);
        Task<ResultValue> Login(UserLoginRequest request);
        Task<ResultValue> Update(UserUpdateRequest request);
        Task<ResultValue> UpdatePassword(UserPasswordRequest request);
        Task<ResultValue> GetById();
        Task<ResultValue> SendEmailWithCodeToResetPassword(UserSendEmailRequest request);
        Task<ResultValue> ResetPassword(UserResetPasswordRequest request);
        Task<ResultValue> GetFamilyMembersByUserId();
        Task<ResultValue> GetFamilyMemberByUserId(Guid familyId);
        Task<ResultValue> InsertFamilyMember(FamilyMemberRequest request);
        Task<ResultValue> UpdateFamilyMember(Guid familyId, FamilyMemberRequest request);
        Task<ResultValue> ActiveInactiveFamilyMember(Guid familyId);
        Task<ResultValue> DeleteFamilyMember(Guid familyId);
    }
}
