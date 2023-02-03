using FinanceControl.Application.Services.User.Model;
using FinanceControl.Application.Services.Wallet.Model;
using FinanceControl.Extensions.BaseRepository;
using FinanceControl.WebApi.Extensions.Context;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
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
                Password = u.Password
            })
            .FirstOrDefaultAsync();

        return result;
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
                Password = u.Password
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
    #endregion
}