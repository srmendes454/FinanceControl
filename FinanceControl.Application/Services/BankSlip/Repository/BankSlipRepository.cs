using FinanceControl.Application.Services.BankSlip.Model;
using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Extensions.BaseRepository;
using FinanceControl.WebApi.Extensions.Context;
using MongoDB.Driver;
using Serilog;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using FinanceControl.Extensions.Paginated;
using System.Linq;

namespace FinanceControl.Application.Services.BankSlip.Repository
{
    public class BankSlipRepository : BaseRepository<BankSlipModel>
    {
        #region [ Fields ]

        public IMongoCollection<BankSlipModel> GetBankSlipCollection() => GetMongoCollection();

        #endregion

        #region [ Constructor ]

        public BankSlipRepository(IContextMongoDBDatabase mongoDb, ILogger logger) : base(mongoDb, logger, "BankSlip")
        {
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Obtem um Boleto por Id
        /// </summary>
        /// <param name="bankSlipId"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        public async Task<BankSlipModel> GetById(Guid bankSlipId, Guid walletId)
        {
            var filter = Builders<BankSlipModel>.Filter
                .Where(p => p.BankSlipId.Equals(bankSlipId)
                            && p.Wallet.WalletId.Equals(walletId)
                            && p.Active.Equals(true));

            var sort = Builders<BankSlipModel>.Sort
                .Ascending(x => x.CreationDate);

            var result = await GetBankSlipCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(bs => new BankSlipModel
                {
                    BankSlipId = bs.BankSlipId,
                    Name = bs.Name,
                    ExpirationDay = bs.ExpirationDay,
                    Active = bs.Active
                })
                .FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Obtem todos os Boletos paginado
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<PaginatedResponse<BankSlipModel>> GetAll(Guid walletId, string search, int take, int skip)
        {
            var filter = Builders<BankSlipModel>.Filter;
            var filters = new List<FilterDefinition<BankSlipModel>>();

            FilterDefinition<BankSlipModel> mainFilter;
            mainFilter = filter.Where(bs => bs.Wallet.WalletId.Equals(walletId)
                                           && bs.Active.Equals(true));

            if (search != null)
                filters.Add(filter.Where(bs => bs.Name.ToLower().Contains(search.ToLower())));

            if (filters.Count > 0)
                foreach (var filterDefinition in filters)
                    mainFilter &= filterDefinition;

            var sort = Builders<BankSlipModel>.Sort
                .Ascending(x => x.Name);

            var result = await GetBankSlipCollection()
                .Aggregate()
                .Match(mainFilter)
                .Sort(sort)
                .Project(bs => new BankSlipModel
                {
                    BankSlipId = bs.BankSlipId,
                    Name = bs.Name,
                    ExpirationDay = bs.ExpirationDay,
                    Active = bs.Active
                })
                .ToListAsync();

            var records = result.Skip((skip - 1) * take).Take(take);
            var newResult = new PaginatedResponse<BankSlipModel>
            {
                Records = records.ToList(),
                Total = result.Count
            };

            return newResult;
        }

        /// <summary>
        /// Atualiza os dados de um Boleto
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async void Update(Guid walletId, BankSlipModel model)
        {
            var filter = Builders<BankSlipModel>.Filter
                .Where(bs => bs.Wallet.WalletId.Equals(walletId)
                            && bs.BankSlipId.Equals(model.BankSlipId)
                            && bs.Active.Equals(true));

            var update = Builders<BankSlipModel>.Update
                .Set(bs => bs.Name, model.Name)
                .Set(bs => bs.ExpirationDay, model.ExpirationDay)
                .Set(bs => bs.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
        }

        /// <summary>
        /// Exclui um Boleto
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="bankSlipId"></param>
        /// <returns></returns>
        public async void Delete(Guid walletId, Guid bankSlipId)
        {
            var filter = Builders<BankSlipModel>.Filter
                .Where(bs => bs.BankSlipId.Equals(bankSlipId)
                            && bs.Wallet.WalletId.Equals(walletId));

            await DeleteOneAsync(filter);
        }

        #endregion
    }
}
