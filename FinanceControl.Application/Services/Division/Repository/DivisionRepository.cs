using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Domain.Entities;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseRepository;
using FinanceControl.Infra.Context;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace FinanceControl.Application.Services.Division.Repository
{
    public class DivisionRepository : BaseRepository<DivisionModel>, IDivisionRepository
    {
        #region [ Fields ]

        public IMongoCollection<DivisionModel> GetDivisionCollection() => GetMongoCollection();

        #endregion

        #region [ Constructor ]

        public DivisionRepository(IContextMongoDBDatabase mongoDb, IAppSettings appSettings) : base(mongoDb, appSettings, "Division")
        {

        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Obtem uma Reparticao por Id
        /// </summary>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        public async Task<DivisionModel> GetById(Guid divisionId)
        {
            var filter = Builders<DivisionModel>.Filter
                .Where(d => d.DivisionId.Equals(divisionId)
                            && d.Active.Equals(true));

            var sort = Builders<DivisionModel>.Sort
                .Ascending(x => x.CreationDate);

            var result = await GetDivisionCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(d => new DivisionModel
                {
                    DivisionId = d.DivisionId,
                    Name = d.Name,
                    Color = d.Color,
                    Percent = d.Percent
                })
                .FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Obtem todas as Divisões paginadas
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<PaginatedResponse<DivisionModel>> GetAll(Guid walletId, string search, int take, int skip)
        {
            var filter = Builders<DivisionModel>.Filter;
            var filters = new List<FilterDefinition<DivisionModel>>();

            FilterDefinition<DivisionModel> mainFilter;
            mainFilter = filter.Where(d => d.Wallet.WalletId.Equals(walletId)
                                           && d.Active.Equals(true));

            if (search != null)
                filters.Add(filter.Where(d => d.Name.ToLower().Contains(search.ToLower())));

            if (filters.Count > 0)
                foreach (var filterDefinition in filters)
                    mainFilter &= filterDefinition;

            var sort = Builders<DivisionModel>.Sort
                .Ascending(d => d.Name);

            var result = await GetDivisionCollection()
                .Aggregate()
                .Match(mainFilter)
                .Sort(sort)
                .Project(d => new DivisionModel
                {
                    DivisionId = d.DivisionId,
                    Name = d.Name,
                    Color = d.Color,
                    Percent = d.Percent,
                    Limits = d.Limits
                })
                .ToListAsync();

            var records = result.Skip((skip - 1) * take).Take(take);
            var newResult = new PaginatedResponse<DivisionModel>
            {
                Records = [.. records],
                Total = result.Count
            };

            return newResult;
        }

        /// <summary>
        /// Atualiza os dados de uma Repartição
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Update(DivisionModel model)
        {
            var filter = Builders<DivisionModel>.Filter
                .Where(x => x.DivisionId.Equals(model.DivisionId)
                            && x.Active.Equals(true));

            var update = Builders<DivisionModel>.Update
                .Set(rec => rec.Name, model.Name)
                .Set(rec => rec.Color, model.Color)
                .Set(rec => rec.Percent, model.Percent)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
        }

        /// <summary>
        /// Exclui um Cartão
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public async Task Delete(Guid divisionId)
        {
            var filter = Builders<DivisionModel>.Filter
                .Where(x => x.DivisionId.Equals(divisionId));

            await DeleteOneAsync(filter);
        }

        #region [ Limits ]

        /// <summary>
        /// Obtem os Limites de uma Reparticao
        /// </summary>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        public async Task<DivisionModel> GetLimitsById(Guid divisionId)
        {
            var filter = Builders<DivisionModel>.Filter
                .Where(d => d.DivisionId.Equals(divisionId)
                            && d.Active.Equals(true));

            var sort = Builders<DivisionModel>.Sort
                .Ascending(x => x.CreationDate);

            var result = await GetDivisionCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(d => new DivisionModel
                {
                    DivisionId = d.DivisionId,
                    Limits = d.Limits
                })
                .FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Insere ou Atualiza os Limites de uma Repartição
        /// </summary>
        /// <param name="divisionId"></param>
        /// <param name="limits"></param>
        /// <returns></returns>
        public async Task UpdateLimit(Guid divisionId, List<LimitModel> limits)
        {
            var filter = Builders<DivisionModel>.Filter
                .Where(x => x.DivisionId.Equals(divisionId)
                            && x.Active.Equals(true));

            var update = Builders<DivisionModel>.Update
                .Set(rec => rec.Limits, limits)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
        }
        #endregion

        #endregion
    }
}
