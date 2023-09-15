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
using FinanceControl.Application.Services.Transactions.Model.Enum;
using FinanceControl.Application.Extensions.Utils.Email;
using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Extensions.BaseRepository;

namespace FinanceControl.Application.Services.Transactions.Service
{
    public class TransactionsService : BaseService
    {
        #region [ Fields ]

        private readonly IEmail _email;

        #endregion

        #region [ Constructor ]

        public TransactionsService(IAppSettings appSettings, ILogger logger,
            Guid currentUserId, IEmail email) : base(logger: logger, appSettings: appSettings,
            currentUserId: currentUserId)
        {
            _email = email;
        }

        #endregion

        #region [ Messages ]

        private const string CardNotFound = "Cartão não encontrado";
        private const string UserNotFound = "Usuário não encontrado";
        private const string TypeCannotBeNull = "Tipo não pode ser vazio";
        private const string TransactionNotFound = "Transação não encontrada";
        private const string Transaction = "Transação";
        private const string SubjectEmail = "Controle Financeiro | Você foi marcado em uma transação";
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
                if (user == null)
                    return ErrorResponse(UserNotFound);

                using var repository = new TransactionsRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
                var model = new TransactionsModel(
                    request.Name,
                    request.DatePurchase,
                    Enum.Parse<TransactionsCashFlow>(request.CashFlow),
                    Enum.Parse<TransactionsType>(request.Type),
                    new RepetitionModel(request.Repetition.QuantityInstallment, request.Repetition.CurrentInstallment, request.Repetition.ValueInstallment)
                );

                switch (model.Type)
                {
                    case TransactionsType.CREDIT_CARD:
                        {
                            using var cardRepository = new CardRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
                            var card = await cardRepository.GetById(request.Id, request.WalletId);
                            if (card == null)
                                return ErrorResponse(CardNotFound);

                            if (string.IsNullOrEmpty(request.AssignedEmail))
                                model.Assigned = new AssignedModel(userId, "@Eu", user.Email);
                            else
                            {
                                var userAssigned = await useRepository.GetByEmail(request.AssignedEmail);
                                if (userAssigned != null)
                                    model.Assigned = new AssignedModel(userAssigned.UserId, userAssigned.Name, userAssigned.Email);
                                else
                                {
                                    var template = _email.TemplateTransactionNotification(user.Name, model.Name, model.Repetition.ValueInstallment, card.Name, card.Type.GetEnumDescription());
                                    var emailSend = _email.Send(request.AssignedEmail, SubjectEmail, template);
                                    if (!emailSend)
                                        return ErrorResponse(Message.SEND_EMAIL_FAIL.GetEnumDescription());
                                }
                            }

                            model.PaymentDetails = new PaymentDetailsModel(card.CardId, card.Name);
                            AddRepetition(request, repository, card, model);
                        }
                        break;
                }

                return SuccessResponse(Transaction, Message.SUCCESSFULLY_ADDED.GetEnumDescription());
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

        private static async void AddRepetition(TransactionsInsertRequest request, TransactionsRepository repository, CardModel card, TransactionsModel model)
        {
            var closingDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, card.ClosingDay);
            var dateExpiration = card.DateExpiration > DateTime.Now.Day
                ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, card.DateExpiration)
                : new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, card.DateExpiration);
            if (request.DatePurchase <= closingDay)
            {
                for (int i = 0; i < request.Repetition.QuantityInstallment; i++)
                {
                    model.Repetition.CurrentInstallment = request.Repetition.CurrentInstallment++;
                    model.ExpirationDate = dateExpiration.AddMonths(i);
                    await repository.InsertOneAsync(model);
                }
            }
            else
            {
                for (int i = 1; i <= request.Repetition.QuantityInstallment; i++)
                {
                    model.Repetition.CurrentInstallment = request.Repetition.CurrentInstallment++;
                    model.ExpirationDate = dateExpiration.AddMonths(i);
                    await repository.InsertOneAsync(model);
                }
            }
        }

        #endregion
    }
}
