using FinanceControl.Application.Services.Transactions.Model;
using FinanceControl.Extensions.BaseRepository;
using FinanceControl.WebApi.Extensions.Context;
using MongoDB.Driver;
using Serilog;

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



        #endregion
    }
}
