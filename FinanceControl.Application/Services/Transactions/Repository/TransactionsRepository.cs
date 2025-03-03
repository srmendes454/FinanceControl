using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseRepository;
using FinanceControl.Infra.Context;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Transactions.Repository
{
    public class TransactionsRepository : BaseRepository<TransactionsModel>, ITransactionsRepository
    {
        #region [ Fields ]
        public IMongoCollection<TransactionsModel> GetTransactionCollection() => GetMongoCollection();
        #endregion

        #region [ Constructor ]

        public TransactionsRepository(IContextMongoDBDatabase mongoDb, IAppSettings appSettings) : base(mongoDb, appSettings, "Transaction")
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
        public async Task<PaginatedResponse<TransactionsModel>> GetAllByPaymentId(Guid paymentId, Guid assignedId, string search, string type, int year, int month, int take, int skip)
        {
            var filter = Builders<TransactionsModel>.Filter;
            var filters = new List<FilterDefinition<TransactionsModel>>();

            FilterDefinition<TransactionsModel> mainFilter;
            mainFilter = filter.Where(t => t.PaymentDetails.Id.Equals(paymentId)
                                           && t.Active.Equals(true));

            if (!string.IsNullOrEmpty(search))
                filters.Add(filter.Where(x => x.Name.ToLower().Contains(search.ToLower())));

            if (assignedId != Guid.Empty)
                filters.Add(filter.Where(x => x.Assigned.AssignedId.Equals(assignedId)));

            if (!string.IsNullOrEmpty(type))
                filters.Add(filter.Where(x => x.Type.ToString() == type));

            if (month > 0 && year > 0)
            {
                var yearMonthReference = $"{year:0000}/{month:00}";
                filters.Add(filter.Where(x => x.YearMonthReference.Equals(yearMonthReference)));
            }

            if (filters.Count > 0)
                foreach (var filterDefinition in filters)
                    mainFilter &= filterDefinition;

            var sort = Builders<TransactionsModel>.Sort
                .Ascending(x => x.ExpirationDate);

            var result = await GetTransactionCollection()
                .Aggregate()
                .Match(mainFilter)
                .Sort(sort)
                .Project(t => new TransactionsModel
                {
                    TransactionId = t.TransactionId,
                    Name = t.Name,
                    CashFlow = t.CashFlow,
                    ExpenseType = t.ExpenseType,
                    DatePurchase = t.DatePurchase,
                    ExpirationDate = t.ExpirationDate,
                    YearMonthReference = t.YearMonthReference,
                    Repetition = t.Repetition,
                    Type = t.Type,
                    PaymentDetails = t.PaymentDetails,
                    Assigned = t.Assigned,
                    Value = t.Value,
                    Installment = t.Installment
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
                .Ascending(x => x.ExpirationDate);

            var result = await GetTransactionCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(t => new TransactionsModel
                {
                    TransactionId = t.TransactionId,
                    Name = t.Name,
                    CashFlow = t.CashFlow,
                    ExpenseType = t.ExpenseType,
                    DatePurchase = t.DatePurchase,
                    Repetition = t.Repetition,
                    Type = t.Type,
                    PaymentDetails = t.PaymentDetails,
                    Assigned = t.Assigned,
                    Value = t.Value,
                    Installment = t.Installment,
                    CreatedBy = t.CreatedBy
                })
                .FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Obtem Transação Salarial, Provento ou Pró-Labore
        /// </summary>
        /// <param name="yearMonthReference"></param>
        /// <returns></returns>
        public async Task<double> GetTransactionSalary(string yearMonthReference)
        {
            var filter = Builders<TransactionsModel>.Filter
                .Where(t => t.YearMonthReference.Equals(yearMonthReference)
                        && (t.ExpenseType.Equals(ExpenseType.SALARY) || t.ExpenseType.Equals(ExpenseType.PRO_LABORE) || t.ExpenseType.Equals(ExpenseType.INCOME))
                            && t.Active.Equals(true));

            var sort = Builders<TransactionsModel>.Sort
                .Ascending(x => x.ExpirationDate);

            var query = await GetTransactionCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(t => new TransactionsModel
                {
                    Value = t.Value,
                    Repetition = t.Repetition
                })
                .ToListAsync();

            var result = Math.Round(query.Sum(t => t.Value ?? t.Repetition.ValueInstallment), 2);
            return result;
        }

        /// <summary>
        /// Obtem Transação por Tipo
        /// </summary>
        /// <param name="yearMonthReference"></param>
        /// <param name="expensesType"></param>
        /// <returns></returns>
        public async Task<List<TransactionsModel>> GetTransactionByExpenseType(string yearMonthReference, List<ExpenseType> expensesType)
        {
            var filter = Builders<TransactionsModel>.Filter
                .Where(t => t.YearMonthReference.Equals(yearMonthReference)
                        && expensesType.Contains(t.ExpenseType)
                            && t.Active.Equals(true));

            var sort = Builders<TransactionsModel>.Sort
                .Ascending(x => x.ExpirationDate);

            var result = await GetTransactionCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(t => new TransactionsModel
                {
                    ExpenseType = t.ExpenseType,
                    Value = t.Value,
                    Repetition = t.Repetition
                })
                .ToListAsync();

            return result;
        }

        /// <summary>
        /// Obtem a Transação por Id e Data
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<TransactionsModel> GetByIdAndDate(Guid transactionId, int year, int month)
        {
            var yearMonthReference = $"{year:0000}/{month:00}";
            var filter = Builders<TransactionsModel>.Filter
                .Where(t => t.TransactionId == transactionId
                        && t.YearMonthReference.Equals(yearMonthReference)
                        && t.Active.Equals(true));

            var record = await GetTransactionCollection()
                .Aggregate()
                .Match(filter)
                .Project(t => new TransactionsModel
                {
                    TransactionId = t.TransactionId,
                    Name = t.Name,
                    CashFlow = t.CashFlow,
                    ExpenseType = t.ExpenseType,
                    DatePurchase = t.DatePurchase,
                    ExpirationDate = t.ExpirationDate,
                    Repetition = t.Repetition,
                    Type = t.Type,
                    PaymentDetails = t.PaymentDetails,
                    Assigned = t.Assigned,
                    Value = t.Value,
                    Installment = t.Installment,
                    CreatedBy = t.CreatedBy
                })
                .FirstOrDefaultAsync();

            return record;
        }

        /// <summary>
        /// Obtem as Transações por CardId e Data
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<List<TransactionsModel>> GetAllByCardIdAndDate(Guid cardId, int year, int month)
        {
            var yearMonthReference = $"{year:0000}/{month:00}";
            var filter = Builders<TransactionsModel>.Filter
                .Where(t => t.PaymentDetails.Id == cardId
                            && t.YearMonthReference.Equals(yearMonthReference)
                            && t.Active.Equals(true));

            var result = await GetTransactionCollection()
                .Aggregate()
                .Match(filter)
                .Project(t => new TransactionsModel
                {
                    TransactionId = t.TransactionId,
                    Name = t.Name,
                    DatePurchase = t.DatePurchase,
                    Repetition = t.Repetition,
                    Value = t.Value
                })
                .ToListAsync();

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
                .Ascending(x => x.ExpirationDate);

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
                    YearMonthReference = t.YearMonthReference,
                    ExpirationDate = t.ExpirationDate
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
                        && x.YearMonthReference.Equals(model.YearMonthReference)
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
        /// Exclui uma Transação
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task Delete(Guid transactionId, int year, int month)
        {
            var yearMonthReference = $"{year:0000}/{month:00}";
            var filter = Builders<TransactionsModel>.Filter
                .Where(t => t.TransactionId == transactionId
                        && t.YearMonthReference.Equals(yearMonthReference));

            await DeleteOneAsync(filter);
        }

        /// <summary>
        /// Exclui varias Transações
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task DeleteTransactions(Guid transactionId)
        {
            var filter = Builders<TransactionsModel>.Filter
                .Where(t => t.TransactionId == transactionId);

            await DeleteManyAsync(filter);
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

        /// <summary>
        /// Move as Transações
        /// </summary>
        /// <param name="transactions"></param>
        /// <returns></returns>
        public async Task UpdateMove(Guid transactionId, string yearMonthReference, TransactionsModel transaction)
        {
            var filter = Builders<TransactionsModel>.Filter
                .Where(x => x.TransactionId.Equals(transactionId)
                            && x.YearMonthReference.Equals(yearMonthReference)
                            && x.Repetition != null
                            && x.Repetition.CurrentInstallment.Equals(transaction.Repetition.CurrentInstallment)
                            && x.Active.Equals(true));

            var update = Builders<TransactionsModel>.Update
                .Set(rec => rec.ExpirationDate, transaction.ExpirationDate)
                .Set(rec => rec.YearMonthReference, transaction.YearMonthReference)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
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
