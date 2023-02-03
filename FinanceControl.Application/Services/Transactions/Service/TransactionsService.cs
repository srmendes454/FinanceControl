using System;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.BaseService;
using FinanceControl.Application.Extensions.Enum;
using FinanceControl.Application.Services.Cards.DTO_s;
using FinanceControl.Application.Services.Cards.Model.Enum;
using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Application.Services.Cards.Repository;
using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Application.Services.Transactions.Model;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Extensions.AppSettings;
using FinanceControl.Extensions.Enum;
using Serilog;
using FinanceControl.Application.Services.Transactions.Repository;

namespace FinanceControl.Application.Services.Transactions.Service
{
    public class TransactionsService : BaseService
    {
        #region [ Constructor ]

        public TransactionsService(IAppSettings appSettings, ILogger logger,
            Guid currentUserId) : base(logger: logger, appSettings: appSettings,
            currentUserId: currentUserId)
        {

        }

        #endregion

        #region [ Messages ]

        private const string CardNotFound = "CARD_NOT_FOUND";
        private const string TypeCannotBeNull = "TYPE_CANNOT_BE_NULL";
        private const string TransactionNotFound = "TRANSACTION_NOT_FOUND";
        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Serviço para inserir uma Transação
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Insert(TransactionsInsertRequest request)
        {
            try
            {
                if (request == null)
                    return ErrorResponse(Message.INVALID_OBJECT.ToString());

                if (string.IsNullOrWhiteSpace(request.Type))
                    return ErrorResponse(TypeCannotBeNull);

                var userId = GetCurrentUserId();
                if (request.WalletId == Guid.Empty && userId == Guid.Empty && request.Id == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.ToString());

                var model = _mapper.Map<TransactionsModel>(request);
                using var repository = new TransactionsRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

                switch (request.Type)
                {
                    case "CREDIT_CARD":
                        {
                            using var cardRepository = new CardRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
                            var card = await cardRepository.GetById(request.Id, request.WalletId);
                            if (card == null)
                                return ErrorResponse(CardNotFound);

                            model.PaymentDetails = new PaymentDetailsModel
                            {
                                Id = card.CardId,
                                Name = card.Name,
                                Color = card.Color
                            };

                            for (int i = 0; i < request.Repetition.QuantityInstallment; i++)
                            {
                                model.Repetition.CurrentInstallment = request.Repetition.CurrentInstallment++;
                                await repository.InsertOneAsync(model);
                            }
                        }
                        break;
                }

                return SuccessResponse("TRANSACTION", Message.SUCCESSFULLY_ADDED.ToString());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }
        #endregion

        #region [ Private Methods ]



        #endregion
    }
}
