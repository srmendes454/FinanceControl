using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Domain.Entities;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseRepository;
using FinanceControl.Infra.Context;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Investment.Repository
{
    public class InvestmentRepository : BaseRepository<InvestmentModel>, IInvestmentRepository
    {
        #region [ Fields ]

        public IMongoCollection<InvestmentModel> GetInvestmentCollection() => GetMongoCollection();

        #endregion

        #region [ Constructor ]

        public InvestmentRepository(IContextMongoDBDatabase mongoDb, IAppSettings appSettings) : base(mongoDb, appSettings, "Investment")
        {
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Obtém um Investimento por Id
        /// </summary>
        /// <param name="investmentId"></param>
        /// <returns></returns>
        public async Task<InvestmentModel> GetById(Guid investmentId)
        {
            var filter = Builders<InvestmentModel>.Filter
                .Where(p => p.InvestmentId.Equals(investmentId)
                            && p.Active.Equals(true));

            var sort = Builders<InvestmentModel>.Sort
                .Ascending(x => x.CreationDate);

            var result = await GetInvestmentCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(p => new InvestmentModel
                {
                    InvestmentId = p.InvestmentId,
                    Name = p.Name,
                    Type = p.Type,
                    Color = p.Color,
                    MonthlyProfitability = p.MonthlyProfitability
                })
                .FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Obtém todos os Investimentos paginados e filtrados
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PaginatedResponse<InvestmentModel>> GetAll(Guid walletId, string search, int take, int skip)
        {
            var filter = Builders<InvestmentModel>.Filter;
            var filters = new List<FilterDefinition<InvestmentModel>>();

            FilterDefinition<InvestmentModel> mainFilter;
            mainFilter = filter.Where(p => p.Wallet.WalletId.Equals(walletId)
                                           && p.Active.Equals(true));

            if (search != null)
                filters.Add(filter.Where(p => p.Name.ToLower().Contains(search.ToLower())));

            if (filters.Count > 0)
                foreach (var filterDefinition in filters)
                    mainFilter &= filterDefinition;

            var sort = Builders<InvestmentModel>.Sort
                .Ascending(x => x.Name);

            var result = await GetInvestmentCollection()
                .Aggregate()
                .Match(mainFilter)
                .Sort(sort)
                .Project(p => new InvestmentModel
                {
                    InvestmentId = p.InvestmentId,
                    Name = p.Name,
                    Type = p.Type,
                    Color = p.Color,
                    MonthlyProfitability = p.MonthlyProfitability
                })
                .ToListAsync();

            var records = result.Skip((skip - 1) * take).Take(take);
            var newResult = new PaginatedResponse<InvestmentModel>
            {
                Records = records.ToList(),
                Total = result.Count
            };

            return newResult;
        }
        
        /// <summary>
        /// Atualiza os dados de um Investimento
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Update(InvestmentModel model)
        {
            var filter = Builders<InvestmentModel>.Filter
                .Where(p => p.InvestmentId.Equals(model.InvestmentId)
                            && p.Active.Equals(true));

            var update = Builders<InvestmentModel>.Update
                .Set(p => p.Name, model.Name)
                .Set(p => p.Color, model.Color)
                .Set(p => p.Type, model.Type)
                .Set(p => p.MonthlyProfitability, model.MonthlyProfitability)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
        }

        /// <summary>
        /// Deleta um Investimento
        /// </summary>
        /// <param name="investmentId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Delete(Guid investmentId)
        {
            var filter = Builders<InvestmentModel>.Filter
                .Where(p => p.InvestmentId.Equals(investmentId));

            await DeleteOneAsync(filter);
        }

        #endregion
    }
}
