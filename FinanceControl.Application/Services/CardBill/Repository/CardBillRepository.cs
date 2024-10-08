using FinanceControl.Application.Services.CardBill.Model;
using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Extensions.BaseRepository;
using FinanceControl.WebApi.Extensions.Context;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.CardBill.Repository
{
    public class CardBillRepository : BaseRepository<CardBillModel>
    {
        #region [ Fields ]
        public IMongoCollection<CardBillModel> GetCardBillCollection() => GetMongoCollection();

        #endregion

        #region [ Constructor ]

        public CardBillRepository(IContextMongoDBDatabase mongoDb, ILogger logger) : base(logger: logger,
            mongoDb: mongoDb,
            collectionName: "CardBill")
        {
                
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Obtem uma Fatura de um mês por Cartão
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<CardBillModel> GetById(Guid cardId, int month)
        {
            var filter = Builders<CardBillModel>.Filter
                .Where(cb => cb.Card.CardId.Equals(cardId)
                            && cb.ExpirationDate.Month.Equals(month)
                            && cb.Active.Equals(true));

            var sort = Builders<CardBillModel>.Sort
                .Ascending(x => x.CreationDate);

            var result = await GetCardBillCollection()
                .Aggregate()
                .Match(filter)
                .Sort(sort)
                .Project(cb => new CardBillModel
                {
                    CardBillId = cb.CardBillId,
                    ExpirationDate = cb.ExpirationDate,
                    TotalValue = cb.TotalValue,
                    Status = cb.Status,
                    Transactions = cb.Transactions
                })
                .FirstOrDefaultAsync();

            return result;
        }

        #endregion
    }
}
