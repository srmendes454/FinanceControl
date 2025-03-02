using FinanceControl.Domain.Entities;
using FinanceControl.Infra.BaseRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.User.Repository
{
    public interface IUserRepository : IBaseRepository<UserModel>
    {
        Task<bool> UserExistByEmail(string email);
        Task<UserModel> GetByEmail(string email);
        Task<string> GetNameById(Guid userId);
        Task<UserModel> GetById(Guid userId);
        Task<UserModel> GetDataPartialById(Guid userId);
        Task Update(Guid userId, UserModel model);
        Task UpdateCode(Guid userId, ResetPasswordModel resetPassword);
        Task UpdatePassword(Guid userId, UserModel model);
        Task<List<FamilyMemberModel>> GetFamilyMembersByUserId(Guid userId);
        Task<FamilyMemberModel> GetFamilyMemberByUserId(Guid userId, Guid familyId);
        Task UpdateFamilyMembers(Guid userId, UserModel model);
    }
}
