using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Domain.Entities;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseRepository;
using FinanceControl.Infra.Context;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Pix.Repository
{
    public class PixRepository : BaseRepository<PixModel>, IPixRepository
    {
        #region [ Fields ]

        public IMongoCollection<PixModel> GetPixCollection() => GetMongoCollection();

        #endregion

        #region [ Constructor ]

        public PixRepository(IContextMongoDBDatabase mongoDb, IAppSettings appSettings) : base(mongoDb, appSettings, "Pix")
        {
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Obtem um Pix por Id
        /// </summary>
        /// <param name="pixId"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        public async Task<PixModel> GetById(Guid pixId)
        {
            var filter = Builders<PixModel>.Filter
                .Where(p => p.PixId.Equals(pixId)
                            && p.Active.Equals(true));

            var sort = Builders<PixModel>.Sort
                .Ascending(x => x.CreationDate);

            var result = await GetPixCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(p => new PixModel
                {
                    PixId = p.PixId,
                    Name = p.Name,
                    LinkedAccount = p.LinkedAccount,
                    Type = p.Type,
                    Color = p.Color,
                    Active = p.Active
                })
                .FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Obtem todos os Pixs paginado
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<PaginatedResponse<PixModel>> GetAll(Guid walletId, string search, int take, int skip)
        {
            var filter = Builders<PixModel>.Filter;
            var filters = new List<FilterDefinition<PixModel>>();

            FilterDefinition<PixModel> mainFilter;
            mainFilter = filter.Where(p => p.Wallet.WalletId.Equals(walletId)
                                           && p.Active.Equals(true));

            if (search != null)
                filters.Add(filter.Where(p => p.Name.ToLower().Contains(search.ToLower())));

            if (filters.Count > 0)
                foreach (var filterDefinition in filters)
                    mainFilter &= filterDefinition;

            var sort = Builders<PixModel>.Sort
                .Ascending(x => x.Name);

            var result = await GetPixCollection()
                .Aggregate()
                .Match(mainFilter)
                .Sort(sort)
                .Project(p => new PixModel
                {
                    PixId = p.PixId,
                    Name = p.Name,
                    LinkedAccount = p.LinkedAccount,
                    Type = p.Type,
                    Color = p.Color,
                    Active = p.Active
                })
                .ToListAsync();

            var records = result.Skip((skip - 1) * take).Take(take);
            var newResult = new PaginatedResponse<PixModel>
            {
                Records = records.ToList(),
                Total = result.Count
            };

            return newResult;
        }

        /// <summary>
        /// Atualiza os dados de um Pix
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Update(PixModel model)
        {
            var filter = Builders<PixModel>.Filter
                .Where(p => p.PixId.Equals(model.PixId)
                            && p.Active.Equals(true));

            var update = Builders<PixModel>.Update
                .Set(p => p.Name, model.Name)
                .Set(p => p.Color, model.Color)
                .Set(p => p.Type, model.Type)
                .Set(p => p.LinkedAccount, model.LinkedAccount)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
        }

        /// <summary>
        /// Exclui um Pix
        /// </summary>
        /// <param name="pixId"></param>
        /// <returns></returns>
        public async Task Delete(Guid pixId)
        {
            var filter = Builders<PixModel>.Filter
                .Where(p => p.PixId.Equals(pixId));

            await DeleteOneAsync(filter);
        }

        #endregion
    }
}
