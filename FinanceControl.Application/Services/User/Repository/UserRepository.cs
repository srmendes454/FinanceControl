using FinanceControl.Application.Services.User.Model;
using FinanceControl.Application.Services.Wallet.Model;
using FinanceControl.Extensions.BaseRepository;
using FinanceControl.WebApi.Extensions.Context;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace FinanceControl.Application.Services.User.Repository;

public class UserRepository : BaseRepository<UserModel>
{
    #region [ Fields ]
    public IMongoCollection<UserModel> GetUserCollection() => GetMongoCollection();
    #endregion

    #region [ Constructor ]
    public UserRepository(IContextMongoDBDatabase mongoDb, ILogger logger) : base(logger: logger,
        mongoDb: mongoDb,
        collectionName: "User")
    {

    }
    #endregion

    #region [ Public Methods ]

    /// <summary>
    /// Obtem um User por Email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<UserModel> GetByEmail(string email)
    {
        var filter = Builders<UserModel>.Filter
            .Where(p => p.Email.Equals(email) && p.Active.Equals(true));

        var sort = Builders<UserModel>.Sort
            .Ascending(x => x.CreationDate);

        var result = await GetUserCollection()
            .Aggregate()
            .Match(filter)
            .Sort(sort)
            .Project(u => new UserModel
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Password = u.Password,
                ResetPassword = u.ResetPassword
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Verifica se já existe conta cadastrada pelo email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<bool> UserExistByEmail(string email)
    {
        var filter = Builders<UserModel>.Filter
            .Where(p => p.Email.Equals(email));

        var record = await GetUserCollection()
            .Aggregate()
            .Match(filter)
            .Project(u => new UserModel { Email = u.Email })
            .AnyAsync();

        return record;
    }

    /// <summary>
    /// Obtem um User por Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<UserModel> GetById(Guid userId)
    {
        var filter = Builders<UserModel>.Filter
            .Where(p => p.UserId.Equals(userId)
                        && p.Active.Equals(true));

        var sort = Builders<UserModel>.Sort
            .Ascending(x => x.CreationDate);

        var result = await GetUserCollection()
            .Aggregate()
            .Match(filter)
            .Sort(sort)
            .Project(u => new UserModel
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                CellPhone = u.CellPhone,
                Occupation = u.Occupation,
                Thumbnail = u.Thumbnail,
                Password = u.Password,
                FamilyMembers = u.FamilyMembers
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Atualiza os dados de um Usuário
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task Update(Guid userId, UserModel model)
    {
        var filter = Builders<UserModel>.Filter
            .Where(x => x.UserId.Equals(userId)
                        && x.Active.Equals(true));

        var update = Builders<UserModel>.Update
            .Set(rec => rec.Name, model.Name)
            .Set(rec => rec.CellPhone, model.CellPhone)
            .Set(rec => rec.Occupation, model.Occupation)
            .Set(rec => rec.Thumbnail, model.Thumbnail)
            .Set(p => p.UpdateDate, DateTime.UtcNow);

        await UpdateOneAsync(update, filter);
    }

    /// <summary>
    /// Insere o código de validação para redefinição de senha
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="resetPassword"></param>
    /// <returns></returns>
    public async Task UpdateCode(Guid userId, ResetPasswordModel resetPassword)
    {
        var filter = Builders<UserModel>.Filter
            .Where(x => x.UserId.Equals(userId)
                        && x.Active.Equals(true));

        var update = Builders<UserModel>.Update
            .Set(rec => rec.ResetPassword, resetPassword)
            .Set(p => p.UpdateDate, DateTime.UtcNow);

        await UpdateOneAsync(update, filter);
    }

    /// <summary>
    /// Atualiza a senha de um Usuário
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task UpdatePassword(Guid userId, UserModel model)
    {
        var filter = Builders<UserModel>.Filter
            .Where(x => x.UserId.Equals(userId)
                        && x.Active.Equals(true));

        var update = Builders<UserModel>.Update
            .Set(rec => rec.Password, model.Password)
            .Set(p => p.UpdateDate, DateTime.UtcNow);

        await UpdateOneAsync(update, filter);
    }

    #region [ Family Members ]

    /// <summary>
    /// Obtem os Membros Familiares do Usuário
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<FamilyMemberModel>> GetFamilyMembersByUserId(Guid userId)
    {
        var filter = Builders<UserModel>.Filter
            .Where(p => p.UserId.Equals(userId)
                        && p.Active.Equals(true));

        var sort = Builders<UserModel>.Sort
            .Ascending(x => x.CreationDate);

        var query = await GetUserCollection()
            .Aggregate()
            .Match(filter)
            .Sort(sort)
            .Project(u => new UserModel
            {
                UserId = u.UserId,
                FamilyMembers = u.FamilyMembers
            })
            .FirstOrDefaultAsync();

        var result = query?.FamilyMembers?.Where(f => f.Active.Equals(true)).ToList();
        return result;
    }

    /// <summary>
    /// Obtem um Membro Familiar 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="familyId"></param>
    /// <returns></returns>
    public async Task<FamilyMemberModel> GetFamilyMemberByUserId(Guid userId, Guid familyId)
    {
        var filter = Builders<UserModel>.Filter
            .Where(p => p.UserId.Equals(userId)
                        && p.Active.Equals(true));

        var sort = Builders<UserModel>.Sort
            .Ascending(x => x.CreationDate);

        var query = await GetUserCollection()
            .Aggregate()
            .Match(filter)
            .Sort(sort)
            .Project(u => new UserModel
            {
                UserId = u.UserId,
                FamilyMembers = u.FamilyMembers
            })
            .FirstOrDefaultAsync();

        var result = query?.FamilyMembers?.FirstOrDefault(f => f.FamilyId.Equals(familyId) && f.Active.Equals(true));
        return result;
    }

    /// <summary>
    /// Insere ou Atualiza os Membros Familiares do Usuário
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task UpdateFamilyMembers(Guid userId, UserModel model)
    {
        var filter = Builders<UserModel>.Filter
            .Where(x => x.UserId.Equals(userId)
                        && x.Active.Equals(true));

        var update = Builders<UserModel>.Update
            .Set(rec => rec.FamilyMembers, model.FamilyMembers)
            .Set(p => p.UpdateDate, DateTime.UtcNow);

        await UpdateOneAsync(update, filter);
    }
    #endregion
    #endregion
}