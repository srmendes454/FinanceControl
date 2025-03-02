using FinanceControl.Domain.Entities;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseRepository;
using FinanceControl.Infra.Context;
using MongoDB.Driver;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.CardBill.Repository
{
    public class CardBillRepository : BaseRepository<CardBillModel>, ICardBillRepository
    {
        #region [ Fields ]
        public IMongoCollection<CardBillModel> GetCardBillCollection() => GetMongoCollection();

        #endregion

        #region [ Constructor ]

        public CardBillRepository(IContextMongoDBDatabase mongoDb, IAppSettings appSettings) : base(mongoDb, appSettings, "CardBill")
        {
                
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Obtem a Fatura por Id
        /// </summary>
        /// <param name="cardBillId"></param>
        /// <returns></returns>
        public async Task<CardBillModel> GetById(Guid cardBillId)
        {
            var filter = Builders<CardBillModel>.Filter
                .Where(cb => cb.CardBillId.Equals(cardBillId)
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
                    YearMonthReference = cb.YearMonthReference,
                    ExpirationDate = cb.ExpirationDate,
                    TotalValue = cb.TotalValue,
                    Status = cb.Status,
                    Card = cb.Card
                })
                .FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Atualiza os dados de um Fatura
        /// </summary>
        /// <param name="cardBillId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Update(Guid cardBillId, CardBillModel model)
        {
            var filter = Builders<CardBillModel>.Filter
                .Where(x => x.CardBillId.Equals(cardBillId)
                            && x.Active.Equals(true));

            var update = Builders<CardBillModel>.Update
                .Set(rec => rec.Status, model.Status)
                .Set(rec => rec.TotalValue, model.TotalValue)
                .Set(rec => rec.ExpirationDate, model.ExpirationDate)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
        }

        /// <summary>
        /// Atualiza os dados de pagamento da Fatura
        /// </summary>
        /// <param name="cardBillId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task UpdatePay(Guid cardBillId, CardBillModel model)
        {
            var filter = Builders<CardBillModel>.Filter
                .Where(x => x.CardBillId.Equals(cardBillId)
                            && x.Active.Equals(true));

            var update = Builders<CardBillModel>.Update
                .Set(rec => rec.Status, model.Status)
                .Set(rec => rec.AmountPaid, model.AmountPaid)
                .Set(rec => rec.RemainderOfPayment, model.RemainderOfPayment)
                .Set(p => p.UpdateDate, DateTime.UtcNow);

            await UpdateOneAsync(update, filter);
        }

        #endregion
    }
}
