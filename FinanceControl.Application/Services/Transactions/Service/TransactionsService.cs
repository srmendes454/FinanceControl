using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Application.Extensions.Utils.Email;
using FinanceControl.Application.Services.AccountBank.Repository;
using FinanceControl.Application.Services.BankSlip.Repository;
using FinanceControl.Application.Services.Cards.Repository;
using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Application.Services.Transactions.DTO_s.Response;
using FinanceControl.Application.Services.Transactions.Repository;
using FinanceControl.Application.Services.User.Repository;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Transactions.Service
{
    public class TransactionsService : BaseService<TransactionsService>, ITransactionsService
    {
        #region [ Fields ]

        private readonly ITransactionsRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IBankSlipRepository _bankSlipRepository;
        private readonly IAccountBankRepository _accountBankRepository;
        private readonly IEmail _email;

        #endregion

        #region [ Constructor ]

        public TransactionsService(IAppSettings appSettings, 
            ITransactionsRepository repository,
            IUserRepository userRepository,
            ICardRepository cardRepository,
            IBankSlipRepository bankSlipRepository,
            IAccountBankRepository accountBankRepository,
            IEmail email) : base(appSettings)
        {
            _repository = repository;
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _bankSlipRepository = bankSlipRepository;
            _accountBankRepository = accountBankRepository;
            _email = email;
        }

        #endregion

        #region [ Messages ]

        private const string CardNotFound = "Cartão não encontrado";
        private const string BankSlipNotFound = "Boleto Bancário não encontrado";
        private const string AccountBankNotFound = "Conta Bancária não encontrada";
        private const string TransactionNotFound = "Transação não encontrada";
        private const string TransactionsNotFound = "Transações não encontradas";
        private const string Transaction = "Transação";
        private const string SubjectEmail = "Controle Financeiro | Você foi marcado em uma transação";
        private const string ExpenseTypeNotFound = "Nenhum Tipo de Despesa foi encontrado";
        private const string CashFlowNotFound = "Nenhum Fluxo de Caixa foi encontrado";
        private const string TransactionsTypeNotFound = "Nenhum Tipo de Transação foi encontrado";
        private const string TransactionEvaluated = "Transação avaliada com sucesso!";

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Serviço para inserir Transação por Cartão
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> InsertToCard(Guid cardId, TransactionsInsertRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (cardId == Guid.Empty || userId == Guid.Empty || request == null)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var user = await _userRepository.GetById(userId);
                if (user == null)
                    return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

                var model = new TransactionsModel(
                    userId,
                    request.Name,
                    request.DatePurchase,
                    request.Installment,
                    TransactionsCashFlow.EXIT,
                    TransactionsType.CREDIT_CARD,
                    Enum.Parse<ExpenseType>(request.ExpenseType)
                );

                var card = await _cardRepository.GetById(cardId);
                if (card == null)
                    return ErrorResponse(CardNotFound);

                AssignedFor(request.AssignedId, user, model, card.Name, card.Type.GetEnumDescription());
                model.PaymentDetails = new PaymentDetailsModel(card.CardId, card.Name);

                if (model.Installment)
                {
                    model.Repetition = new RepetitionModel(request.Repetition.QuantityInstallment, request.Repetition.CurrentInstallment, request.Repetition.ValueInstallment);

                    var transactions = new List<TransactionsModel>();
                    var transactionsRepetition = model.AddRepetitionCard(request.Repetition.QuantityInstallment, request.Repetition.CurrentInstallment, card.ClosingDay, card.ExpirationDay);
                    transactions.AddRange(transactionsRepetition);

                    await _repository.InsertManyAsync(transactions);
                }
                else
                {
                    var expirationDate = new DateTime(model.DatePurchase.Year, model.DatePurchase.Month, card.ExpirationDay);
                    model.LoadData(request.Value.Value, expirationDate, card.ClosingDay);
                    
                    await _repository.InsertOneAsync(model);
                }

                return SuccessResponse(Transaction, Message.SUCCESSFULLY_ADDED_F.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para inserir Transação por Boleto
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> InsertToBankSlip(Guid bankSlipId, TransactionsInsertRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (bankSlipId == Guid.Empty || userId == Guid.Empty || request == null)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var user = await _userRepository.GetById(userId);
                if (user == null)
                    return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

                var model = new TransactionsModel(
                    userId,
                    request.Name,
                    request.DatePurchase,
                    request.Installment,
                    TransactionsCashFlow.EXIT,
                    TransactionsType.BANK_SLIP,
                    Enum.Parse<ExpenseType>(request.ExpenseType)
                );

                var bankSlip = await _bankSlipRepository.GetById(bankSlipId);
                if (bankSlip == null)
                    return ErrorResponse(BankSlipNotFound);

                AssignedFor(request.AssignedId, user, model, bankSlip.Name, model.Type.GetEnumDescription());

                model.PaymentDetails = new PaymentDetailsModel(bankSlip.BankSlipId, bankSlip.Name);

                if (model.Installment)
                {
                    model.Repetition = new RepetitionModel(request.Repetition.QuantityInstallment, request.Repetition.CurrentInstallment, request.Repetition.ValueInstallment);

                    var transactions = new List<TransactionsModel>();
                    var transactionsRepetition = model.AddRepetitionBankSlip(request.Repetition.QuantityInstallment, request.Repetition.CurrentInstallment, bankSlip.ExpirationDay);
                    transactions.AddRange(transactionsRepetition);

                    await _repository.InsertManyAsync(transactions);
                }
                else
                {
                    var expirationDate = new DateTime(model.DatePurchase.Year, model.DatePurchase.Month, bankSlip.ExpirationDay);
                    model.LoadData(request.Value.Value, expirationDate);

                    await _repository.InsertOneAsync(model);
                }

                return SuccessResponse(Transaction, Message.SUCCESSFULLY_ADDED_F.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para inserir Transação por Conta Bancária
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> InsertToAccountBank(Guid accountBankId, TransactionsInsertRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (accountBankId == Guid.Empty || userId == Guid.Empty || request == null)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var user = await _userRepository.GetById(userId);
                if (user == null)
                    return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

                var model = new TransactionsModel(
                    userId,
                    request.Name,
                    request.DatePurchase,
                    request.Installment,
                    Enum.Parse<TransactionsCashFlow>(request.CashFlow),
                    Enum.Parse<TransactionsType>(request.Type),
                    Enum.Parse<ExpenseType>(request.ExpenseType)
                );

                var accountBank = await _accountBankRepository.GetById(accountBankId);
                if (accountBank == null)
                    return ErrorResponse(AccountBankNotFound);

                AssignedFor(request.AssignedId, user, model, accountBank.Name, model.Type.GetEnumDescription());
                model.PaymentDetails = new PaymentDetailsModel(accountBank.AccountBankId, accountBank.Name);

                model.LoadData(request.Value.Value, request.DatePurchase);

                await _repository.InsertOneAsync(model);

                return SuccessResponse(Transaction, Message.SUCCESSFULLY_ADDED_F.GetEnumDescription());
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

                var list = await _repository.GetAllByPaymentId(paymentId, assignedId, search, type, year, month, take, skip);
                if (list == null)
                    return SuccessResponse(new PaginatedResponse<TransactionsListResponse> { Records = new List<TransactionsListResponse>() });

                var record = list.Records.Select(t => new TransactionsListResponse
                {
                    TransactionId = t.TransactionId,
                    Id = t.PaymentDetails.Id,
                    Name = t.Name,
                    Assigned = t.Assigned.Name,
                    Value = t.Installment ? t.Repetition.ValueInstallment : t.Value.Value,
                    Installment = t.Installment ? $"{t.Repetition.CurrentInstallment}/{t.Repetition.NumberInstallments}" : "1/1",
                    CashFlow = t.CashFlow.GetEnumDescription(),
                    ExpenseType = t.ExpenseType.GetEnumDescription(),
                    Type = t.Type.GetEnumDescription(),
                    DatePurchase = $"{t.DatePurchase:dd} {t.DatePurchase:MMMM} {t.DatePurchase:yy}",
                    ExpirationDate = $"{t.ExpirationDate:dd} {t.ExpirationDate:MMMM} {t.ExpirationDate:yy}",
                    YearMonthReference = t.YearMonthReference
                });

                var result = new PaginatedResponse<TransactionsListResponse>
                {
                    Records = [.. record],
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
                if (transactionId == Guid.Empty || year == 0 || month == 0)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var record = await _repository.GetByIdAndDate(transactionId, year, month);
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
                if (transactionId == Guid.Empty || request == null || GetCurrentUserId() == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.ToString());

                var user = await _userRepository.GetById(GetCurrentUserId());
                if (user == null)
                    return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

                if (request.Installment && request.UpdateAll)
                {
                    var transactions = await _repository.GetTransactionsById(transactionId);
                    if (transactions.Count == 0)
                        return ErrorResponse(TransactionsNotFound);

                    transactions = await UpdateAll(transactions, request);

                    foreach (var transaction in transactions)
                        await _repository.Update(transactionId, transaction);
                }
                else
                {
                    var transaction = await _repository.GetByIdAndDate(transactionId, year, month);
                    if (transaction == null)
                        return ErrorResponse(TransactionNotFound);

                    transaction.Update(request.Name, request.DatePurchase, request.Installment, Enum.Parse<TransactionsCashFlow>(request.CashFlow), Enum.Parse<ExpenseType>(request.ExpenseType));

                    if (request.AssignedId != Guid.Empty && request.AssignedId != transaction.Assigned.AssignedId)
                        AssignedForUpdate(request.AssignedId, user, transaction);

                    if (transaction.Installment)
                    {
                        var repetition = request.Repetition;
                        transaction.Repetition.Update(repetition.QuantityInstallment, repetition.CurrentInstallment, repetition.ValueInstallment);
                    }
                    else
                        transaction.Value = request.Value;

                    await _repository.Update(transactionId, transaction);
                }

                return SuccessResponse(Transaction, Message.SUCCESSFULLY_UPDATED_F.GetEnumDescription());
            }
            catch (Exception ex) { return ErrorResponse(ex); }
        }

        /// <summary>
        /// Serviço para Excluir uma Transação
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<ResultValue> Delete(Guid transactionId, int year, int month, bool deleteAll)
        {
            try
            {
                if (deleteAll)
                {
                    await _repository.DeleteTransactions(transactionId);
                }
                else
                {
                    if (transactionId == Guid.Empty || year == 0 || month == 0)
                        return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                    await _repository.Delete(transactionId, year, month);
                }

                return SuccessResponse(Transaction, Message.SUCCESSFULLY_DELETED_F.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Mover a Transação 
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task<ResultValue> MoveTransaction(Guid transactionId, bool next)
        {
            try
            {
                if (transactionId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var transactions = await _repository.GetTransactionsById(transactionId);
                if (transactions.Count == 0)
                    return ErrorResponse(TransactionsNotFound);

                var addMonths = next ? 1 : -1;
                foreach (var transaction in transactions)
                {
                    var yearMonthReference = transaction.YearMonthReference;
                    transaction.ExpirationDate = transaction.ExpirationDate.AddMonths(addMonths);
                    transaction.YearMonthReference = transaction.ExpirationDate.ToString("yyyy/MM");

                    await _repository.UpdateMove(transactionId, yearMonthReference, transaction);
                }

                return SuccessResponse(Transaction, Message.SUCCESSFULLY_UPDATED_F.GetEnumDescription());
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
                var list = await _repository.ListAssignedTransactions(GetCurrentUserId(), search, take, skip);
                if (list == null)
                    return SuccessResponse(new PaginatedResponse<TransactionsAssignedToMeResponse> { Records = new List<TransactionsAssignedToMeResponse>() });

                using var useRepository = new UserRepository(_appSettings.GetMongoDb(), _appSettings);
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
                var user = await _userRepository.GetById(userId);
                if (user == null)
                    return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

                var transaction = await _repository.GetById(request.TransactionId);
                if (transaction == null)
                    return ErrorResponse(TransactionNotFound);

                if (request.Approved)
                {
                    var card = await _cardRepository.GetById(request.CardId);
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
                    var userCreated = await _userRepository.GetDataPartialById(transaction.CreatedBy.Value);

                    var transactions = await _repository.GetTransactionsById(request.TransactionId);
                    if (transactions.Any())
                        transaction.Assigned = new AssignedModel(userCreated.UserId, "@Eu", userCreated.Email);
                }
                await _repository.UpdateAllAssigned(request.TransactionId, transaction.Assigned);

                return SuccessResponse(TransactionEvaluated);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion

        #endregion

        #region [ Private Methods ]

        private void AssignedFor(Guid assignedId, UserModel user, TransactionsModel model, string namePayment, string typePayment)
        {
            if (assignedId == Guid.Empty)
                model.Assigned = new AssignedModel(user.UserId, "@Eu", user.Email);
            else
            {
                var familyMember = user.FamilyMembers?.FirstOrDefault(fm => fm.UserId.Equals(assignedId));
                if (familyMember != null)
                    SignedBy(user, familyMember, model, namePayment, typePayment);
            }
        }

        private void AssignedForUpdate(Guid assignedId, UserModel user, TransactionsModel model)
        {
            if (assignedId == user.UserId)
                model.Assigned.Update(user.UserId, "@Eu", user.Email);
            else
            {
                var familyMember = user.FamilyMembers?.FirstOrDefault(fm => fm.UserId.Equals(assignedId));
                if (familyMember != null)
                {
                    if (familyMember.UserId != Guid.Empty)
                        model.Assigned = new AssignedModel(familyMember.UserId, familyMember.Name, familyMember.Email);
                    else
                    {
                        model.Assigned = new AssignedModel(familyMember.FamilyId, familyMember.Name, familyMember.Email);
                    }
                }
            }
        }

        private void SignedBy(UserModel user, FamilyMemberModel familyMember, TransactionsModel model, string namePayment, string typePayment)
        {
            if (familyMember.UserId != Guid.Empty)
                model.Assigned = new AssignedModel(familyMember.UserId, familyMember.Name, familyMember.Email);
            else
            {
                model.Assigned = new AssignedModel(familyMember.FamilyId, familyMember.Name, familyMember.Email);
                try
                {
                    var template = _email.TemplateTransactionNotification(user.Name, model.Name, model.Repetition.ValueInstallment, namePayment, typePayment);
                    var emailSend = _email.Send(familyMember.Email, SubjectEmail, template);
                }
                catch (Exception) { }
            }
        }

        private async Task<List<TransactionsModel>> UpdateAll(List<TransactionsModel> transactions, TransactionsUpdateRequest request)
        {
            var repetition = request.Repetition;
            var quantityInstallment = transactions.FirstOrDefault().Repetition.NumberInstallments;

            transactions.ForEach(transaction =>
            {
                transaction.Update(
                    request.Name,
                    request.DatePurchase,
                    request.Installment,
                    Enum.Parse<TransactionsCashFlow>(request.CashFlow),
                    Enum.Parse<ExpenseType>(request.ExpenseType)
                );

                transaction.Repetition.Update(repetition.QuantityInstallment, repetition.CurrentInstallment++, repetition.ValueInstallment);
            });

            if (request.Repetition.QuantityInstallment != quantityInstallment)
            {
                var quantityInstallmentUpdate = request.Repetition.QuantityInstallment - quantityInstallment;
                if (quantityInstallmentUpdate < 0)
                {
                    quantityInstallmentUpdate = Math.Abs(quantityInstallmentUpdate);
                    while (quantityInstallmentUpdate > 0)
                    {
                        var lastTransaction = transactions.LastOrDefault();
                        transactions.Remove(lastTransaction);

                        await _repository.Delete(lastTransaction.TransactionId, lastTransaction.ExpirationDate.Year, lastTransaction.ExpirationDate.Month);
                        quantityInstallmentUpdate--;
                    }
                }
                else
                {
                    var iteration = 1;
                    var lastTransaction = transactions.LastOrDefault();
                    var currentInstallment = lastTransaction.Repetition.CurrentInstallment;
                    while (currentInstallment < request.Repetition.QuantityInstallment)
                    {
                        currentInstallment++;
                        var newTransaction = new TransactionsModel
                        {
                            TransactionId = lastTransaction.TransactionId,
                            Name = lastTransaction.Name,
                            Active = true,
                            CashFlow = lastTransaction.CashFlow,
                            CreatedBy = lastTransaction.CreatedBy,
                            CreationDate = lastTransaction.CreationDate,
                            DatePurchase = lastTransaction.DatePurchase,
                            ExpenseType = lastTransaction.ExpenseType,
                            Installment = lastTransaction.Installment,
                            PaymentDetails = lastTransaction.PaymentDetails,
                            Assigned = lastTransaction.Assigned,
                            Type = lastTransaction.Type,
                            Value = lastTransaction.Value,
                            ExpirationDate = lastTransaction.ExpirationDate.AddMonths(iteration),
                            YearMonthReference = lastTransaction.ExpirationDate.AddMonths(iteration).ToString("yyyy/MM"),
                            Repetition = lastTransaction.Installment ? new RepetitionModel(lastTransaction.Repetition.NumberInstallments, currentInstallment, lastTransaction.Repetition.ValueInstallment) : null
                        };

                        await _repository.InsertOneAsync(newTransaction);
                        iteration++;
                    }
                }
            }

            return transactions;
        }

        #endregion
    }
}
