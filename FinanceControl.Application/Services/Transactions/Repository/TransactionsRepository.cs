using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Application.Services.Transactions.Model;
using FinanceControl.Extensions.BaseRepository;
using FinanceControl.Extensions.Paginated;
using FinanceControl.WebApi.Extensions.Context;
using MongoDB.Driver;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace FinanceControl.Application.Services.Transactions.Repository
{
    public class TransactionsRepository : BaseRepository<TransactionsModel>
    {
        #region [ Fields ]
        public IMongoCollection<TransactionsModel> GetTransactionCollection() => GetMongoCollection();
        #endregion

        #region [ Constructor ]

        public TransactionsRepository(IContextMongoDBDatabase mongoDb, ILogger logger) : base(logger: logger,
            mongoDb: mongoDb,
            collectionName: "Transaction")
        {
            
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Obtem todas as transações paginadas com filtros
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="assignedId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<PaginatedResponse<TransactionsModel>> GetAllByPaymentId(Guid paymentId, Guid assignedId, string search, int take, int skip)
        {
            var filter = Builders<TransactionsModel>.Filter;
            var filters = new List<FilterDefinition<TransactionsModel>>();

            FilterDefinition<TransactionsModel> mainFilter;
            mainFilter = filter.Where(t => t.PaymentDetails.Id.Equals(paymentId)
                                           && t.Active.Equals(true));

            if (search != null)
                filters.Add(filter.Where(x => x.Name.ToLower().Contains(search.ToLower())));

            if (assignedId != Guid.Empty)
                filters.Add(filter.Where(x => x.Assigned.AssignedId.Equals(assignedId)));

            if (filters.Count > 0)
                foreach (var filterDefinition in filters)
                    mainFilter &= filterDefinition;

            var sort = Builders<TransactionsModel>.Sort
                .Ascending(x => x.Name);

            var result = await GetTransactionCollection()
                .Aggregate()
                .Match(mainFilter)
                .Sort(sort)
                .Project(t => new TransactionsModel
                {
                    TransactionId = t.TransactionId,
                    Name = t.Name,
                    CashFlow = t.CashFlow,
                    DatePurchase = t.DatePurchase,
                    Repetition = t.Repetition,
                    Type = t.Type,
                    PaymentDetails = t.PaymentDetails,
                    Assigned = t.Assigned
                })
                .ToListAsync();

            var records = result.Skip((skip - 1) * take).Take(take);
            var newResult = new PaginatedResponse<TransactionsModel>
            {
                Records = records.ToList(),
                Total = result.Count
            };

            return newResult;
        }

        /// <summary>
        /// Obtem a Transação por Id
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<TransactionsModel> GetById(Guid transactionId)
        {
            var filter = Builders<TransactionsModel>.Filter
                .Where(t => t.TransactionId.Equals(transactionId)
                            && t.Active.Equals(true));

            var sort = Builders<TransactionsModel>.Sort
                .Ascending(x => x.Name);

            var result = await GetTransactionCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(t => new TransactionsModel
                {
                    TransactionId = t.TransactionId,
                    Name = t.Name,
                    CashFlow = t.CashFlow,
                    DatePurchase = t.DatePurchase,
                    Repetition = t.Repetition,
                    Type = t.Type,
                    PaymentDetails = t.PaymentDetails,
                    Assigned = t.Assigned,
                    CreatedBy = t.CreatedBy
                })
                .FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Obtem todas as Transações por Id
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<List<TransactionsModel>> GetTransactionsById(Guid transactionId)
        {
            var filter = Builders<TransactionsModel>.Filter
                .Where(t => t.TransactionId.Equals(transactionId)
                            && t.Active.Equals(true));

            var sort = Builders<TransactionsModel>.Sort
                .Ascending(x => x.Name);

            var result = await GetTransactionCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(t => new TransactionsModel
                {
                    TransactionId = t.TransactionId,
                    Name = t.Name,
                    CashFlow = t.CashFlow,
                    DatePurchase = t.DatePurchase,
                    Repetition = t.Repetition,
                    Type = t.Type,
                    PaymentDetails = t.PaymentDetails,
                    Assigned = t.Assigned
                })
                .ToListAsync();

            return result;
        }

        /// <summary>
        /// Atualiza os dados de uma Transação
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Update(Guid transactionId, TransactionsModel model)
        {
            var filter = Builders<TransactionsModel>.Filter
                .Where(x => x.TransactionId.Equals(transactionId)
                            && x.Active.Equals(true));

            var update = Builders<TransactionsModel>.Update
                .Set(rec => rec.Name, model.Name)
                .Set(rec => rec.CashFlow, model.CashFlow)
                .Set(rec => rec.DatePurchase, model.DatePurchase)
                .Set(rec => rec.Repetition, model.Repetition)
                .Set(rec => rec.Type, model.Type)
                .Set(rec => rec.PaymentDetails, model.PaymentDetails)
                .Set(rec => rec.Assigned, model.Assigned)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
        }

        /// <summary>
        /// Atualiza a atribuição de uma Transação
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task UpdateAssigned(Guid transactionId, TransactionsModel model)
        {
            var filter = Builders<TransactionsModel>.Filter
                .Where(x => x.TransactionId.Equals(transactionId)
                            && x.Active.Equals(true));

            var update = Builders<TransactionsModel>.Update
                .Set(rec => rec.Assigned, model.Assigned)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
        }

        /// <summary>
        /// Atualiza as atribuições de uma Transação
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="assignedModel"></param>
        /// <returns></returns>
        public async Task UpdateAllAssigned(Guid transactionId, AssignedModel assignedModel)
        {
            var filter = Builders<TransactionsModel>.Filter
                .Where(x => x.TransactionId.Equals(transactionId)
                            && x.Active.Equals(true));

            var update = Builders<TransactionsModel>.Update
                .Set(rec => rec.Assigned, assignedModel)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateManyAsync(update, filter);
        }

        #region [ Duties ]

        /// <summary>
        /// Obtem todas as Transações que me foi atribuido
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<PaginatedResponse<TransactionsModel>> ListAssignedTransactions(Guid userId, string search, int take, int skip)
        {
            var filter = Builders<TransactionsModel>.Filter;
            var filters = new List<FilterDefinition<TransactionsModel>>();

            FilterDefinition<TransactionsModel> mainFilter;
            mainFilter = filter.Where(t => t.Active.Equals(true) && t.Assigned.AssignedId.Equals(userId) && t.Assigned.Name != "@Eu");

            if (search != null)
                filters.Add(filter.Where(x => x.Name.ToLower().Contains(search.ToLower())));

            if (filters.Count > 0)
                foreach (var filterDefinition in filters)
                    mainFilter &= filterDefinition;

            var sort = Builders<TransactionsModel>.Sort
                .Ascending(x => x.Name);

            var result = await GetTransactionCollection()
                .Aggregate()
                .Match(mainFilter)
                .Sort(sort)
                .Project(t => new TransactionsModel
                {
                    TransactionId = t.TransactionId,
                    Name = t.Name,
                    CashFlow = t.CashFlow,
                    DatePurchase = t.DatePurchase,
                    Repetition = t.Repetition,
                    Type = t.Type,
                    CreatedBy = t.CreatedBy
                })
                .ToListAsync();

            var records = result.DistinctBy(_ => _.TransactionId).Skip((skip - 1) * take).Take(take);
            var newResult = new PaginatedResponse<TransactionsModel>
            {
                Records = records.ToList(),
                Total = result.Count
            };

            return newResult;
        }

        #endregion

        #endregion
    }
}
