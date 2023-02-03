using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Extensions.BaseRepository;
using FinanceControl.Extensions.Paginated;
using FinanceControl.WebApi.Extensions.Context;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILogger = Serilog.ILogger;

namespace FinanceControl.Application.Services.Cards.Repository;

public class CardRepository : BaseRepository<CardModel>
{
    #region [ Fields ]
    public IMongoCollection<CardModel> GetCardCollection() => GetMongoCollection();
    #endregion

    #region [ Constructor ]

    public CardRepository(IContextMongoDBDatabase mongoDb, ILogger logger) : base(logger: logger,
        mongoDb: mongoDb,
        collectionName: "Card")
    {

    }

    #endregion

    #region [ Public Methods ]

    /// <summary>
    /// Obtem um cartão por Id
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="walletId"></param>
    /// <returns></returns>
    public async Task<CardModel> GetById(Guid cardId, Guid walletId)
    {
        var filter = Builders<CardModel>.Filter
            .Where(p => p.CardId.Equals(cardId)
                        && p.Wallet.WalletId.Equals(walletId)
                        && p.Active.Equals(true));

        var sort = Builders<CardModel>.Sort
            .Ascending(x => x.CreationDate);

        var result = await GetCardCollection()
            .Aggregate()
            .Match(filter)
            .Sort(sort)
            .Project(c => new CardModel
            {
                CardId = c.CardId,
                Name = c.Name,
                Number = c.Number,
                Payment = c.Payment,
                Status = c.Status,
                PriceTotal = c.PriceTotal,
                DateExpiration = c.DateExpiration,
                Color = c.Color,
                Active = c.Active,
                CardType = c.CardType,
                AvailableLimit = c.AvailableLimit
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Obtem todos os cartões paginado
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="search"></param>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<PaginatedResponse<CardModel>> GetAll(Guid walletId, string search, int take, int skip)
    {
        var filter = Builders<CardModel>.Filter;
        var filters = new List<FilterDefinition<CardModel>>();

        FilterDefinition<CardModel> mainFilter;
        mainFilter = filter.Where(p => p.Wallet.WalletId.Equals(walletId)
                                       && p.Active.Equals(true));

        if (search != null)
            filters.Add(filter.Where(x => x.Name.ToLower().Contains(search.ToLower())));

        if (filters.Count > 0)
            foreach (var filterDefinition in filters)
                mainFilter &= filterDefinition;

        var sort = Builders<CardModel>.Sort
            .Ascending(x => x.Name);

        var result = await GetCardCollection()
            .Aggregate()
            .Match(mainFilter)
            .Sort(sort)
            .Project(c => new CardModel
            {
                CardId = c.CardId,
                Name = c.Name,
                Number = c.Number,
                Payment = c.Payment,
                Status = c.Status,
                PriceTotal = c.PriceTotal,
                DateExpiration = c.DateExpiration,
                Color = c.Color,
                Active = c.Active,
                CardType = c.CardType,
                AvailableLimit = c.AvailableLimit
            })
            .ToListAsync();

        var records = result.Skip((skip - 1) * take).Take(take);
        var newResult = new PaginatedResponse<CardModel>
        {
            Records = records.ToList(),
            Total = result.Count
        };

        return newResult;
    }

    /// <summary>
    /// Atualiza os dados de um Cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task Update(Guid walletId, CardModel model)
    {
        var filter = Builders<CardModel>.Filter
            .Where(x => x.Wallet.WalletId.Equals(walletId)
                        && x.CardId.Equals(model.CardId)
                        && x.Active.Equals(true));

        var update = Builders<CardModel>.Update
            .Set(rec => rec.Name, model.Name)
            .Set(rec => rec.Number, model.Number)
            .Set(rec => rec.Color, model.Color)
            .Set(rec => rec.DateExpiration, model.DateExpiration)
            .Set(rec => rec.AvailableLimit, model.AvailableLimit)
            .Set(rec => rec.CardType, model.CardType)
            .Set(p => p.UpdateDate, DateTime.UtcNow);

        await UpdateOneAsync(update, filter);
    }

    /// <summary>
    /// Atualiza os dados de um Cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <param name="active"></param>
    /// <returns></returns>
    public async Task UpdateActive(Guid walletId, Guid cardId)
    {
        var filter = Builders<CardModel>.Filter
            .Where(x => x.Wallet.WalletId.Equals(walletId)
                        && x.CardId.Equals(cardId)
                        && x.Active.Equals(false));

        var update = Builders<CardModel>.Update
            .Set(rec => rec.Active, true)
            .Set(p => p.UpdateDate, DateTime.Now);

        await UpdateOneAsync(update, filter);
    }

    /// <summary>
    /// Atualiza os dados de um Cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public async Task UpdateInactive(Guid walletId, Guid cardId)
    {
        var filter = Builders<CardModel>.Filter
            .Where(x => x.Wallet.WalletId.Equals(walletId)
                        && x.CardId.Equals(cardId)
                        && x.Active.Equals(true));

        var update = Builders<CardModel>.Update
            .Set(rec => rec.Active, false)
            .Set(p => p.UpdateDate, DateTime.Now);

        await UpdateOneAsync(update, filter);
    }

    #endregion
}