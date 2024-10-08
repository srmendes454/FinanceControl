using FinanceControl.Application.Extensions.BaseService;
using FinanceControl.Application.Extensions.Enum;
using FinanceControl.Application.Extensions.Utils.Email;
using FinanceControl.Application.Extensions.Utils.Repetition;
using FinanceControl.Application.Extensions.Utils.SignedBy;
using FinanceControl.Application.Services.BankSlip.Repository;
using FinanceControl.Application.Services.Cards.Model.Enum;
using FinanceControl.Application.Services.Cards.Repository;
using FinanceControl.Application.Services.Pix.Repository;
using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Application.Services.Transactions.DTO_s.Response;
using FinanceControl.Application.Services.Transactions.Model;
using FinanceControl.Application.Services.Transactions.Model.Enum;
using FinanceControl.Application.Services.Transactions.Repository;
using FinanceControl.Application.Services.User.Model;
using FinanceControl.Application.Services.User.Repository;
using FinanceControl.Extensions.AppSettings;
using FinanceControl.Extensions.Paginated;
using MongoDB.Driver.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Transactions.Service
{
    public class TransactionsService : BaseService
    {
        #region [ Fields ]

        private readonly IEmail _email;
        private readonly IAddRepetition _addRepetition;
        private readonly ISignedBy _signedBy;

        #endregion

        #region [ Constructor ]

        public TransactionsService(IAppSettings appSettings, ILogger logger,
            Guid currentUserId, IEmail email, IAddRepetition addRepetition, ISignedBy signedBy) : base(logger: logger, appSettings: appSettings,
            currentUserId: currentUserId)
        {
            _email = email;
            _addRepetition = addRepetition;
            _signedBy = signedBy;
        }

        #endregion

        #region [ Messages ]

        private const string CardNotFound = "Cartão de Crédito não encontrado";
        private const string DebitNotFound = "Cartão de Débito não encontrado";
        private const string BankSlipNotFound = "Boleto Bancário não encontrado";
        private const string PixNotFound = "Pix não encontrado";
        private const string UserNotFound = "Usuário não encontrado";
        private const string FamilyMemberNotFound = "Membro Familiar não encontrado";
        private const string TypeCannotBeNull = "Tipo não pode ser vazio";
        private const string TransactionNotFound = "Transação não encontrada";
        private const string Transaction = "Transação";
        private const string SubjectEmail = "Controle Financeiro | Você foi marcado em uma transação";
        private const string ExpenseTypeNotFound = "Nenhum Tipo de Despesa foi encontrado";
        private const string CashFlowNotFound = "Nenhum Fluxo de Caixa foi encontrado";
        private const string TransactionsTypeNotFound = "Nenhum Tipo de Transação foi encontrado";

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
                if (string.IsNullOrWhiteSpace(request.Type))
                    return ErrorResponse(TypeCannotBeNull);

                var userId = GetCurrentUserId();
                if (request.WalletId == Guid.Empty && userId == Guid.Empty && request == null && request.Id == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.ToString());

                using var useRepository = new UserRepository(_appSettings.GetMongoDb(), _logger);
                var user = await useRepository.GetById(userId);
                if (user == null)
                    return ErrorResponse(UserNotFound);

                var model = new TransactionsModel(
                    userId,
                    request.Name,
                    request.DatePurchase,
                    request.Installment,
                    Enum.Parse<TransactionsCashFlow>(request.CashFlow),
                    Enum.Parse<TransactionsType>(request.Type),
                    Enum.Parse<ExpenseType>(request.ExpenseType)
                );

                var transactions = new List<TransactionsModel>();
                switch (model.Type)
                {
                    case TransactionsType.CREDIT_CARD:
                        {
                            using var cardRepository = new CardRepository(_appSettings.GetMongoDb(), _logger);
                            var card = await cardRepository.GetById(request.Id, request.WalletId);
                            if (card == null)
                                return ErrorResponse(CardNotFound);

                            if (request.AssignedId == Guid.Empty)
                                model.Assigned = new AssignedModel(user.UserId, "@Eu", user.Email);
                            else
                            {
                                var familyMember = user.FamilyMembers?.FirstOrDefault(fm => fm.UserId.Equals(request.AssignedId));
                                if (familyMember == null)
                                    return ErrorResponse(FamilyMemberNotFound);

                                _signedBy.SignedByCard(user.Name, familyMember, model, card.Name, _email, SubjectEmail);
                            }

                            model.PaymentDetails = new PaymentDetailsModel(card.CardId, card.Name);
                            if (request.Installment)
                            {
                                var repetition = request.Repetition;
                                model.Repetition = new RepetitionModel(repetition.QuantityInstallment, repetition.CurrentInstallment, repetition.ValueInstallment);

                                transactions.AddRange(_addRepetition.AddRepetitionCard(request.Repetition.QuantityInstallment, request.Repetition.CurrentInstallment, card.ClosingDay, card.ExpirationDay, model));
                            }
                            else
                            {
                                model.Value = request.Value;
                                transactions.AddRange(_addRepetition.AddRepetitionCard(1, 1, card.ClosingDay, card.ExpirationDay, model));
                            }
                        }
                        break;

                    case TransactionsType.DEBIT_CARD:
                        {
                            using var cardRepository = new CardRepository(_appSettings.GetMongoDb(), _logger);
                            var card = await cardRepository.GetById(request.Id, request.WalletId);
                            if (card == null)
                                return ErrorResponse(DebitNotFound);

                            if (request.AssignedId == Guid.Empty)
                                model.Assigned = new AssignedModel(user.UserId, "@Eu", user.Email);
                            else
                            {
                                var familyMember = user.FamilyMembers?.FirstOrDefault(fm => fm.UserId.Equals(request.AssignedId));
                                if (familyMember == null)
                                    return ErrorResponse(FamilyMemberNotFound);

                                _signedBy.SignedByCardDebit(user.Name, familyMember, model, card.Name, _email, SubjectEmail);
                            }

                            model.PaymentDetails = new PaymentDetailsModel(card.CardId, card.Name);
                            if (request.Installment)
                            {
                                var repetition = request.Repetition;
                                model.Repetition = new RepetitionModel(repetition.QuantityInstallment, repetition.CurrentInstallment, repetition.ValueInstallment);

                                transactions.AddRange(_addRepetition.AddRepetitionCardDebitAndPix(request.Repetition.QuantityInstallment, request.Repetition.CurrentInstallment, card.ExpirationDay, model));
                            }
                            else
                            {
                                model.Value = request.Value;
                                transactions.AddRange(_addRepetition.AddRepetitionCardDebitAndPix(1, 1, card.ExpirationDay, model));
                            }
                        }
                        break;

                    case TransactionsType.BANK_SLIP:
                        {
                            using var bankSlipRepository = new BankSlipRepository(_appSettings.GetMongoDb(), _logger);
                            var bankSlip = await bankSlipRepository.GetById(request.Id, request.WalletId);
                            if (bankSlip == null)
                                return ErrorResponse(BankSlipNotFound);

                            if (request.AssignedId == Guid.Empty)
                                model.Assigned = new AssignedModel(user.UserId, "@Eu", user.Email);
                            else
                            {
                                var familyMember = user.FamilyMembers?.FirstOrDefault(fm => fm.UserId.Equals(request.AssignedId));
                                if (familyMember == null)
                                    return ErrorResponse(FamilyMemberNotFound);

                                _signedBy.SignedByBankSlip(user.Name, familyMember, model, bankSlip.Name, _email, SubjectEmail);
                            }

                            model.PaymentDetails = new PaymentDetailsModel(bankSlip.BankSlipId, bankSlip.Name);

                            if (request.Installment)
                            {
                                var repetition = request.Repetition;
                                model.Repetition = new RepetitionModel(repetition.QuantityInstallment, repetition.CurrentInstallment, repetition.ValueInstallment);

                                transactions.AddRange(_addRepetition.AddRepetitionBankSlip(request.Repetition.QuantityInstallment, request.Repetition.CurrentInstallment, bankSlip.ExpirationDay, model));
                            }
                            else
                            {
                                model.Value = request.Value;
                                transactions.AddRange(_addRepetition.AddRepetitionBankSlip(1, 1, bankSlip.ExpirationDay, model));
                            }
                        }
                        break;

                    case TransactionsType.PIX:
                        {
                            using var pixRepository = new PixRepository(_appSettings.GetMongoDb(), _logger);
                            var pix = await pixRepository.GetById(request.Id, request.WalletId);
                            if (pix == null)
                                return ErrorResponse(PixNotFound);

                            if (request.AssignedId == Guid.Empty)
                                model.Assigned = new AssignedModel(user.UserId, "@Eu", user.Email);
                            else
                            {
                                var familyMember = user.FamilyMembers?.FirstOrDefault(fm => fm.UserId.Equals(request.AssignedId));
                                if (familyMember == null)
                                    return ErrorResponse(FamilyMemberNotFound);

                                _signedBy.SignedByPix(user.Name, familyMember, model, pix.Name, _email, SubjectEmail);
                            }

                            model.PaymentDetails = new PaymentDetailsModel(pix.PixId, pix.Name);

                            if (request.Installment)
                            {
                                var repetition = request.Repetition;
                                model.Repetition = new RepetitionModel(repetition.QuantityInstallment, repetition.CurrentInstallment, repetition.ValueInstallment);

                                transactions.AddRange(_addRepetition.AddRepetitionCardDebitAndPix(request.Repetition.QuantityInstallment, request.Repetition.CurrentInstallment, pix.ExpirationDay, model));
                            }
                            else
                            {
                                model.Value = request.Value;
                                transactions.AddRange(_addRepetition.AddRepetitionCardDebitAndPix(1, 1, pix.ExpirationDay, model));
                            }
                        }
                        break;
                }

                await new TransactionsRepository(_appSettings.GetMongoDb(), _logger).InsertManyAsync(transactions);
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
        public async Task<ResultValue> GetAllByPaymentId(Guid paymentId, Guid assignedId, string search, string type, int year, int month, int take, int skip)
        {
            try
            {
                if (paymentId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                using var repository = new TransactionsRepository(_appSettings.GetMongoDb(), _logger);
                var list = await repository.GetAllByPaymentId(paymentId, assignedId, search, type, year, month, take, skip);
                if (list == null)
                    return SuccessResponse(new PaginatedResponse<TransactionsListResponse> { Records = new List<TransactionsListResponse>() });

                var record = list.Records.Select(t => new TransactionsListResponse
                {
                    TransactionId = t.TransactionId,
                    Id = t.PaymentDetails.Id,
                    Name = t.Name,
                    Assigned = t.Assigned.Name,
                    Value = t.Installment ? t.Repetition.ValueInstallment : t.Value.Value,
                    Installment = CurrentInstallment(t.Installment, t.ExpirationDate, t.Repetition.CurrentInstallment, t.Repetition.NumberInstallments),
                    CashFlow = t.CashFlow.GetEnumDescription(),
                    ExpenseType = t.ExpenseType.GetEnumDescription(),
                    Type = t.Type.GetEnumDescription(),
                    DatePurchase = $"{t.DatePurchase:dd} {t.DatePurchase:MMMM} {t.DatePurchase:yy}"
                });

                var result = new PaginatedResponse<TransactionsListResponse>
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

        /// <summary>
        /// Serviço para Obter a Transação por Id
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<ResultValue> GetByIdAndDate(Guid transactionId, int year, int month)
        {
            try
            {
                if (transactionId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                using var repository = new TransactionsRepository(_appSettings.GetMongoDb(), _logger);
                var record = await repository.GetByIdAndDate(transactionId, year, month);
                if (record == null)
                    return ErrorResponse(TransactionNotFound);

                var result = new TransactionsResponse
                {
                    TransactionId = record.TransactionId,
                    Id = record.PaymentDetails.Id,
                    Name = record.Name,
                    Value = record.Installment == false ? record.Value.Value : 0,
                    CashFlow = record.CashFlow.GetEnumDescription(),
                    ExpenseType = record.ExpenseType.GetEnumDescription(),
                    Type = record.Type.GetEnumDescription(),
                    DatePurchase = record.DatePurchase,
                    Repetition = _mapper.Map<TransactionsRepetitionResponse>(record.Repetition),
                    Assigned = _mapper.Map<TransactionsAssignedResponse>(record.Assigned)
                };

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Atualizar uma Transação
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Update(Guid transactionId, int year, int month, TransactionsUpdateRequest request)
        {
            try
            {
                if (transactionId == Guid.Empty || request == null)
                    return ErrorResponse(Message.INVALID_OBJECT.ToString());

                using var repository = new TransactionsRepository(_appSettings.GetMongoDb(), _logger);
                var transaction = await repository.GetByIdAndDate(transactionId, year, month);
                if (transaction == null)
                    return ErrorResponse(TransactionNotFound);

                using var useRepository = new UserRepository(_appSettings.GetMongoDb(), _logger);
                var user = await useRepository.GetById(GetCurrentUserId());
                if (user == null)
                    return ErrorResponse(UserNotFound);

                transaction.Update(request.Name, request.DatePurchase, request.Installment, Enum.Parse<TransactionsCashFlow>(request.CashFlow), Enum.Parse<ExpenseType>(request.ExpenseType));
                switch (transaction.Type)
                {
                    case TransactionsType.CREDIT_CARD:
                        {
                            using var cardRepository = new CardRepository(_appSettings.GetMongoDb(), _logger);
                            var card = await cardRepository.GetById(request.Id, request.WalletId);
                            if (card == null)
                                return ErrorResponse(CardNotFound);

                            if (request.AssignedId == Guid.Empty)
                                transaction.Assigned.Update(user.UserId, "@Eu", user.Email);
                            else
                            {
                                var familyMember = user.FamilyMembers?.FirstOrDefault(fm => fm.UserId.Equals(request.AssignedId));
                                if (familyMember == null)
                                    return ErrorResponse(FamilyMemberNotFound);

                                SignedBy(user, familyMember, transaction, card, true);
                            }

                            if (request.Installment)
                            {
                                var repetition = request.Repetition;
                                transaction.Repetition.Update(repetition.QuantityInstallment, repetition.CurrentInstallment, repetition.ValueInstallment);
                            }
                            else
                                transaction.Value = request.Value;
                        }
                        break;

                    case TransactionsType.DEBIT_CARD:
                        {
                            using var cardRepository = new CardRepository(_appSettings.GetMongoDb(), _logger);
                            var card = await cardRepository.GetById(request.Id, request.WalletId);
                            if (card == null)
                                return ErrorResponse(DebitNotFound);

                            if (request.AssignedId == Guid.Empty)
                                transaction.Assigned.Update(user.UserId, "@Eu", user.Email);
                            else
                            {
                                var familyMember = user.FamilyMembers?.FirstOrDefault(fm => fm.UserId.Equals(request.AssignedId));
                                if (familyMember == null)
                                    return ErrorResponse(FamilyMemberNotFound);

                                SignedBy(user, familyMember, transaction, card, true);
                            }

                            if (request.Installment)
                            {
                                var repetition = request.Repetition;
                                transaction.Repetition.Update(repetition.QuantityInstallment, repetition.CurrentInstallment, repetition.ValueInstallment);
                            }
                            else
                                transaction.Value = request.Value;
                        }
                        break;

                    case TransactionsType.BANK_SLIP:
                        {
                            using var bankSlipRepository = new BankSlipRepository(_appSettings.GetMongoDb(), _logger);
                            var bankSlip = await bankSlipRepository.GetById(request.Id, request.WalletId);
                            if (bankSlip == null)
                                return ErrorResponse(BankSlipNotFound);

                            if (request.AssignedId == Guid.Empty)
                                transaction.Assigned.Update(user.UserId, "@Eu", user.Email);
                            else
                            {
                                var familyMember = user.FamilyMembers?.FirstOrDefault(fm => fm.UserId.Equals(request.AssignedId));
                                if (familyMember == null)
                                    return ErrorResponse(FamilyMemberNotFound);

                                SignedBy(user, familyMember, transaction, bankSlip, false);
                            }

                            if (request.Installment)
                            {
                                var repetition = request.Repetition;
                                transaction.Repetition.Update(repetition.QuantityInstallment, repetition.CurrentInstallment, repetition.ValueInstallment);
                            }
                            else
                                transaction.Value = request.Value;
                        }
                        break;

                    case TransactionsType.PIX:
                        {
                            using var pixRepository = new PixRepository(_appSettings.GetMongoDb(), _logger);
                            var pix = await pixRepository.GetById(request.Id, request.WalletId);
                            if (pix == null)
                                return ErrorResponse(PixNotFound);

                            if (request.AssignedId == Guid.Empty)
                                transaction.Assigned.Update(user.UserId, "@Eu", user.Email);
                            else
                            {
                                var familyMember = user.FamilyMembers?.FirstOrDefault(fm => fm.UserId.Equals(request.AssignedId));
                                if (familyMember == null)
                                    return ErrorResponse(FamilyMemberNotFound);

                                SignedBy(user, familyMember, transaction, pix, false);
                            }

                            if (request.Installment)
                            {
                                var repetition = request.Repetition;
                                transaction.Repetition.Update(repetition.QuantityInstallment, repetition.CurrentInstallment, repetition.ValueInstallment);
                            }
                            else
                                transaction.Value = request.Value;
                        }
                        break;

                }

                return SuccessResponse(Transaction, Message.SUCCESSFULLY_ADDED.GetEnumDescription());
            }
            catch (Exception ex) { return ErrorResponse(ex); }
        }

        /// <summary>
        /// Serviço para Excluir uma Transação
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<ResultValue> Delete(Guid transactionId, int year, int month)
        {
            try
            {
                if (transactionId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                using var repository = new TransactionsRepository(_appSettings.GetMongoDb(), _logger);
                await repository.Delete(transactionId, year, month);

                return SuccessResponse(Transaction, Message.SUCCESSFULLY_DELETED.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #region [ List Enuns ]

        /// <summary>
        /// Listagem do Tipo de Despesa
        /// </summary>
        /// <returns></returns>
        public ResultValue ListExpenseType()
        {
            try
            {
                var result = Enum.GetValues<ExpenseType>().GetEnumDescriptionAtributte();
                if (result == null || result.Count <= 0)
                    return ErrorResponse(ExpenseTypeNotFound);

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Listagem do Fluxo de Caixa
        /// </summary>
        /// <returns></returns>
        public ResultValue ListCashFlow()
        {
            try
            {
                var result = Enum.GetValues<TransactionsCashFlow>().GetEnumDescriptionAtributte();
                if (result == null || result.Count <= 0)
                    return ErrorResponse(CashFlowNotFound);

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Listagem de Tipo de Transação
        /// </summary>
        /// <returns></returns>
        public ResultValue ListTransactionsType()
        {
            try
            {
                var result = Enum.GetValues<TransactionsType>().GetEnumDescriptionAtributte();
                if (result == null || result.Count <= 0)
                    return ErrorResponse(TransactionsTypeNotFound);

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion

        #region [ Assigned Transaction ]

        /// <summary>
        /// Obtem todas as Transações que me foi atribuido
        /// </summary>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<ResultValue> ListAssignedTransactions(string search, int take, int skip)
        {
            try
            {
                using var repository = new TransactionsRepository(_appSettings.GetMongoDb(), _logger);
                var list = await repository.ListAssignedTransactions(GetCurrentUserId(), search, take, skip);
                if (list == null)
                    return SuccessResponse(new PaginatedResponse<TransactionsAssignedToMeResponse> { Records = new List<TransactionsAssignedToMeResponse>() });

                using var useRepository = new UserRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
                var record = list.Records.Select(t => new TransactionsAssignedToMeResponse
                {
                    Marked = useRepository.GetNameById(t.CreatedBy.Value).Result,
                    TransactionId = t.TransactionId,
                    Name = t.Name,
                    CashFlow = t.CashFlow.GetEnumDescription(),
                    Type = t.Type.GetEnumDescription(),
                    DatePurchase = $"{t.DatePurchase:dd} {t.DatePurchase:MMMM} {t.DatePurchase:yy}",
                    QuantityInstallment = t.Repetition.NumberInstallments,
                    ValueInstallment = t.Repetition.ValueInstallment
                });

                var result = new PaginatedResponse<TransactionsAssignedToMeResponse>
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

        /// <summary>
        /// Serviço para Avaliar as atribuições em transações
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> EvaluateAssignedTransaction(TransactionsEvaluateAssignedRequest request)
        {
            try
            {
                if (request == null || request.TransactionId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.ToString());

                var userId = GetCurrentUserId();
                using var useRepository = new UserRepository(_appSettings.GetMongoDb(), _logger);
                var user = await useRepository.GetById(userId);
                if (user == null)
                    return ErrorResponse(UserNotFound);

                using var repository = new TransactionsRepository(_appSettings.GetMongoDb(), _logger);
                var transaction = await repository.GetById(request.TransactionId);
                if (transaction == null)
                    return ErrorResponse(TransactionNotFound);

                if (request.Approved)
                {
                    using var cardRepository = new CardRepository(_appSettings.GetMongoDb(), _logger);
                    var card = await cardRepository.GetById(request.CardId, request.WalletId);
                    if (card == null)
                        return ErrorResponse(CardNotFound);

                    transaction.TransactionId = Guid.NewGuid();
                    transaction.Assigned.Name = "@Eu";
                    transaction.PaymentDetails = new PaymentDetailsModel(card.CardId, card.Name);

                    //AddRepetition(transaction.DatePurchase, transaction.Repetition.NumberInstallments, transaction.Repetition.CurrentInstallment, card, transaction, true);
                }
                else
                {
                    //enviar notificação
                    var userCreated = await useRepository.GetDataPartialById(transaction.CreatedBy.Value);

                    var transactions = await repository.GetTransactionsById(request.TransactionId);
                    if (transactions.Any())
                        transaction.Assigned = new AssignedModel(userCreated.UserId, "@Eu", userCreated.Email);
                }
                await repository.UpdateAllAssigned(request.TransactionId, transaction.Assigned);

                return SuccessResponse(Transaction, Message.EVALUATE_TRANSACTION.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion

        #endregion

        #region [ Private Methods ]

        private void SignedBy<T>(UserModel user, FamilyMemberModel familyMember, TransactionsModel model, T typePayment, bool isCard)
        {
            var type = string.Empty;
            var nameProperty = typeof(T).GetProperty("Name");
            var name = nameProperty?.GetValue(typePayment)?.ToString();

            if (isCard)
            {
                var typeProperty = typeof(T).GetProperty("Type");
                var cardType = (CardType)typeProperty?.GetValue(typePayment);
                type = cardType.GetEnumDescription();
            }

            if (familyMember.UserId != Guid.Empty)
                model.Assigned = new AssignedModel(familyMember.UserId, familyMember.Name, familyMember.Email);
            else
            {
                model.Assigned = new AssignedModel(familyMember.FamilyId, familyMember.Name, familyMember.Email);
                try
                {
                    var template = _email.TemplateTransactionNotification(user.Name, model.Name, model.Repetition.ValueInstallment, name, type);
                    var emailSend = _email.Send(familyMember.Email, SubjectEmail, template);
                }
                catch (Exception) { }
            }
        }

        private string CurrentInstallment(bool installment, DateTime expirationDate, int currentInstallment, int numberInstallments)
        {
            var result = "1/1";
            if (!installment)
                return result;

            result = expirationDate > DateTime.Today ? $"{currentInstallment}/{numberInstallments}" : $"{currentInstallment + 1}/{numberInstallments}";
            return result;
        }

        #endregion
    }
}
