using FinanceControl.Application.Services.Wallet.Model;
using FinanceControl.Extensions.BaseRepository;
using FinanceControl.WebApi.Extensions.Context;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Wallet.Repository;

public class WalletRepository : BaseRepository<WalletModel>
{
    #region [ Fields ]

    public IMongoCollection<WalletModel> GetWalletCollection() => GetMongoCollection();

    #endregion

    #region [ Constructor ]

    public WalletRepository(IContextMongoDBDatabase mongoDb, ILogger logger) : base(logger: logger,
        mongoDb: mongoDb,
        collectionName: "Wallet")
    {

    }

    #endregion

    #region [ Public Methods ]

    /// <summary>
    /// Obtem a Carteira por Id
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<WalletModel> GetById(Guid walletId, Guid userId)
    {
        var filter = Builders<WalletModel>.Filter
            .Where(p => p.WalletId.Equals(walletId)
                        && p.User.UserId.Equals(userId)
                        && p.Active.Equals(true));

        var sort = Builders<WalletModel>.Sort
            .Ascending(x => x.CreationDate);

        var result = await GetWalletCollection()
            .Aggregate()
            .Match(filter)
            .Sort(sort)
            .Project(w => new WalletModel
            {
                WalletId = w.WalletId,
                Name = w.Name,
                Color = w.Color
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Obtem as Carteiras do usuário
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<WalletModel>> GetAllByUser(Guid userId)
    {
        var filter = Builders<WalletModel>.Filter
            .Where(p => p.User.UserId.Equals(userId)
                        && p.Active.Equals(true));

        var sort = Builders<WalletModel>.Sort
            .Ascending(x => x.CreationDate);

        var result = await GetWalletCollection()
            .Aggregate()
            .Match(filter)
            .Sort(sort)
            .Project(w => new WalletModel
            {
                WalletId = w.WalletId,
                Name = w.Name,
                Color = w.Color
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Atualiza os dados de uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task Update(Guid walletId, WalletModel model)
    {
        var filter = Builders<WalletModel>.Filter
            .Where(x => x.WalletId.Equals(walletId)
                        && x.Active.Equals(true));

        var update = Builders<WalletModel>.Update
            .Set(rec => rec.Name, model.Name)
            .Set(rec => rec.Color, model.Color)
            .Set(p => p.UpdateDate, model.UpdateDate);

        await UpdateOneAsync(update, filter);
    }

    /// <summary>
    /// Ativa uma uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task UpdateActive(Guid walletId, Guid userId)
    {
        var filter = Builders<WalletModel>.Filter
            .Where(x => x.WalletId.Equals(walletId)
                        && x.User.UserId.Equals(userId)
                        && x.Active.Equals(false));

        var update = Builders<WalletModel>.Update
            .Set(rec => rec.Active, true)
            .Set(p => p.UpdateDate, DateTime.Now);

        await UpdateOneAsync(update, filter);
    }

    /// <summary>
    /// Inativa uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task UpdateInactive(Guid walletId, Guid userId)
    {
        var filter = Builders<WalletModel>.Filter
            .Where(x => x.WalletId.Equals(walletId)
                        && x.User.UserId.Equals(userId)
                        && x.Active.Equals(true));

        var update = Builders<WalletModel>.Update
            .Set(rec => rec.Active, false)
            .Set(p => p.UpdateDate, DateTime.Now);

        await UpdateOneAsync(update, filter);
    }

    /// <summary>
    /// Exclui uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task Delete(Guid walletId, Guid userId)
    {
        var filter = Builders<WalletModel>.Filter
            .Where(x => x.WalletId.Equals(walletId)
                        && x.User.UserId.Equals(userId)
                        && x.Active.Equals(true));

        await DeleteOneAsync(filter);
    }
    #endregion
}