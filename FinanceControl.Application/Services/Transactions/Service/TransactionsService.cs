using FinanceControl.Application.Extensions.BaseService;
using FinanceControl.Application.Services.Cards.Repository;
using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Application.Services.Transactions.DTO_s.Response;
using FinanceControl.Application.Services.Transactions.Model;
using FinanceControl.Application.Services.Transactions.Repository;
using FinanceControl.Application.Services.User.Model;
using FinanceControl.Application.Services.User.Repository;
using FinanceControl.Cards.DTO_s;
using FinanceControl.Extensions.AppSettings;
using FinanceControl.Extensions.Paginated;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.Enum;

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

                using var useRepository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
                var user = await useRepository.GetById(userId);

                var familyMember = new FamilyMemberModel();
                if (request.AssignedId != Guid.Empty)
                    familyMember = await useRepository.GetFamilyMemberByUserId(userId, request.AssignedId);

                using var repository = new TransactionsRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
                var model = _mapper.Map<TransactionsModel>(request);
                switch (request.Type)
                {
                    case "CREDIT_CARD":
                        {
                            using var cardRepository = new CardRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
                            var card = await cardRepository.GetById(request.Id, request.WalletId);
                            if (card == null)
                                return ErrorResponse(CardNotFound);

                            if (familyMember != null)
                                model.Assigned = new AssignedModel
                                {
                                    AssignedId = familyMember?.FamilyId == Guid.Empty ? user.UserId : familyMember.FamilyId,
                                    Name = familyMember?.Name ?? user.Name
                                };

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

        /// <summary>
        /// Serviço para Obter as Transações por Tipo de Pagamento
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="assignedId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<ResultValue> GetAll(Guid paymentId, Guid assignedId, string search, int take, int skip)
        {
            try
            {
                if (paymentId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.ToString());

                using var repository = new TransactionsRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
                var list = await repository.GetAllByPaymentId(paymentId, assignedId, search, take, skip);
                if (list == null)
                    return SuccessResponse(new PaginatedResponse<TransactionsResponse> { Records = new List<TransactionsResponse>() });

                var record = _mapper.Map<List<TransactionsResponse>>(list.Records);
                var result = new PaginatedResponse<TransactionsResponse>
                {
                    Records = record.ToList(),
                    Total = list.Total
                };

                return SuccessResponse(result);
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
