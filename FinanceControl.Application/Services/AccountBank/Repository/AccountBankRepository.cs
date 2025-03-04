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

namespace FinanceControl.Application.Services.AccountBank.Repository
{
    public class AccountBankRepository : BaseRepository<AccountBankModel>, IAccountBankRepository
    {
        #region [ Fields ]

        public IMongoCollection<AccountBankModel> GetAccountBankCollection() => GetMongoCollection();

        #endregion

        #region [ Constructor ]

        public AccountBankRepository(IContextMongoDBDatabase mongoDb, IAppSettings appSettings) : base(mongoDb, appSettings, "AccountBank")
        {
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Obtem uma Conta Bancaria por Id
        /// </summary>
        /// <param name="accountBankId"></param>
        /// <returns></returns>
        public async Task<AccountBankModel> GetById(Guid accountBankId)
        {
            var filter = Builders<AccountBankModel>.Filter
                .Where(p => p.AccountBankId.Equals(accountBankId)
                            && p.Active.Equals(true));

            var sort = Builders<AccountBankModel>.Sort
                .Ascending(x => x.CreationDate);

            var result = await GetAccountBankCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(p => new AccountBankModel
                {
                    AccountBankId = p.AccountBankId,
                    Name = p.Name,
                    Type = p.Type,
                    Color = p.Color,
                    Active = p.Active
                })
                .FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Obtem todos as Contas Bancárias paginado
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<PaginatedResponse<AccountBankModel>> GetAll(Guid walletId, string search, int take, int skip)
        {
            var filter = Builders<AccountBankModel>.Filter;
            var filters = new List<FilterDefinition<AccountBankModel>>();

            FilterDefinition<AccountBankModel> mainFilter;
            mainFilter = filter.Where(p => p.Wallet.WalletId.Equals(walletId)
                                           && p.Active.Equals(true));

            if (search != null)
                filters.Add(filter.Where(p => p.Name.ToLower().Contains(search.ToLower())));

            if (filters.Count > 0)
                foreach (var filterDefinition in filters)
                    mainFilter &= filterDefinition;

            var sort = Builders<AccountBankModel>.Sort
                .Ascending(x => x.Name);

            var result = await GetAccountBankCollection()
                .Aggregate()
                .Match(mainFilter)
                .Sort(sort)
                .Project(p => new AccountBankModel
                {
                    AccountBankId = p.AccountBankId,
                    Name = p.Name,
                    Type = p.Type,
                    Color = p.Color,
                    Active = p.Active
                })
                .ToListAsync();

            var records = result.Skip((skip - 1) * take).Take(take);
            var newResult = new PaginatedResponse<AccountBankModel>
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
        public async Task Update(AccountBankModel model)
        {
            var filter = Builders<AccountBankModel>.Filter
                .Where(p => p.AccountBankId.Equals(model.AccountBankId)
                            && p.Active.Equals(true));

            var update = Builders<AccountBankModel>.Update
                .Set(p => p.Name, model.Name)
                .Set(p => p.Color, model.Color)
                .Set(p => p.Type, model.Type)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
        }

        /// <summary>
        /// Exclui uma Conta Bancária
        /// </summary>
        /// <param name="accountBankId"></param>
        /// <returns></returns>
        public async Task Delete(Guid accountBankId)
        {
            var filter = Builders<AccountBankModel>.Filter
                .Where(p => p.AccountBankId.Equals(accountBankId));

            await DeleteOneAsync(filter);
        }

        #endregion
    }
}
