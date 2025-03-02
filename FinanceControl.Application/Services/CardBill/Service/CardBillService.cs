using FinanceControl.Application.Services.CardBill.DTO_s.Request;
using FinanceControl.Application.Services.CardBill.Repository;
using FinanceControl.Application.Services.Cards.Repository;
using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Application.Services.Transactions.Repository;
using FinanceControl.Application.Services.Transactions.Service;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.CardBill.Service;

public class CardBillService : BaseService<CardBillService>, ICardBillService
{
    #region [ Fields ]

    private readonly ICardBillRepository _repository;
    private readonly ICardRepository _cardRepository;
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly ITransactionsService _transactionsService;

    #endregion

    #region [ Constructor ]
    public CardBillService(IAppSettings appSettings, ICardBillRepository repository, ICardRepository cardRepository, ITransactionsRepository transactionsRepository, ITransactionsService transactionsService) : base(appSettings)
    {
        _repository = repository;
        _cardRepository = cardRepository;
        _transactionsRepository = transactionsRepository;
        _transactionsService = transactionsService;
    }
    #endregion

    #region [ Messages ]

    private const string CardNotFound = "Cartão de Crédito não encontrado";
    private const string CardBillNotFound = "Fatura não encontrada";
    private const string CardBill = "Fatura";
    private const string CardBillGenerate = "gerada com sucesso";
    private const string CardBillPay = "paga com sucesso";
    private const string TransactionsNotFound = "Não foi encontrado dados para gerar Fatura do mês selecionado!";
    private const string PaymentUnavailable = "Fatura ainda não se encontra disponivel para pagamento!";

    #endregion

    #region [ Public Methods ]

    /// <summary>
    /// Serviço para Gerar a Fatura de um mês por Cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    public async Task<ResultValue> GenerateCardBill(Guid cardId, int month, int year)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (cardId == Guid.Empty || month == 0 || year == 0)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var cardCredit = await _cardRepository.GetById(cardId);
            if (cardCredit == null)
                return ErrorResponse(CardNotFound);

            var expirationDate = new DateTime(year, month, cardCredit.ExpirationDay);
            var closingDate = new DateTime(year, month, cardCredit.ClosingDay);

            var transactions = await _transactionsRepository.GetAllByCardIdAndDate(cardId, year, month);
            if (transactions.Count == 0)
                return ErrorResponse(TransactionsNotFound);

            var yearMonthReference = $"{year}/{month}";
            var model = new CardBillModel(userId, expirationDate, yearMonthReference, new CardBillCardCredit(cardCredit.CardId, cardCredit.Name));

            model.Status = ReturnStatusBill(closingDate, expirationDate, model.Status);
            model.TotalValue = transactions.Sum(t => t.Value ?? t.Repetition.ValueInstallment);

            await _repository.InsertOneAsync(model);

            var result = ReturnData(model, transactions);
            return SuccessResponse(result, CardBill, CardBillGenerate);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Atualizar os dados da Fatura
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="cardBillId"></param>
    /// <returns></returns>
    public async Task<ResultValue> UpdateCardBill(Guid cardId, Guid cardBillId, int month, int year)
    {
        try
        {
            if (cardBillId == Guid.Empty || cardId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var cardCredit = await _cardRepository.GetById(cardId);
            if (cardCredit == null)
                return ErrorResponse(CardNotFound);

            var cardBill = await _repository.GetById(cardBillId);
            if (cardBill == null)
                return ErrorResponse(CardBillNotFound);

            var expirationDate = cardBill.ExpirationDate;
            var closingDate = new DateTime(expirationDate.Year, expirationDate.Month, cardCredit.ClosingDay);

            var transactions = await _transactionsRepository.GetAllByCardIdAndDate(cardId, year, month);
            if (transactions.Count == 0)
                return ErrorResponse(TransactionsNotFound);

            if (cardBill.Status != Status.PAID && cardBill.Status != Status.PARTIALLY_PAID)
                cardBill.Status = ReturnStatusBill(closingDate, expirationDate, cardBill.Status);

            cardBill.TotalValue = Math.Round(transactions.Sum(t => t.Value ?? t.Repetition.ValueInstallment), 2);

            await _repository.Update(cardBillId, cardBill);

            var result = ReturnData(cardBill, transactions);
            return SuccessResponse(result, CardBill, Message.SUCCESSFULLY_UPDATED_F.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Efetuar o Pagamento da Fatura
    /// </summary>
    /// <param name="cardBillId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> PayCardBill(Guid cardBillId, PayCardBillRequest request)
    {
        try
        {
            if (cardBillId == Guid.Empty || request == null)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var cardBill = await _repository.GetById(cardBillId);
            if (cardBill == null)
                return ErrorResponse(CardBillNotFound);

            if (cardBill.Status == Status.OPEN)
                return ErrorResponse(PaymentUnavailable);

            if (!request.FullPayment)
            {
                var remainingAmount = cardBill.TotalValue - request.AmountPaid;
                cardBill.Pay(request.FullPayment, cardBill.TotalValue, remainingAmount);
                var transactionRequest = new TransactionsInsertRequest
                {
                    Name = "Restante Fatura Anterior",
                    CashFlow = TransactionsCashFlow.EXIT.ToString(),
                    Value = remainingAmount,
                    Installment = false,
                    DatePurchase = DateTime.UtcNow,
                    ExpenseType = ExpenseType.INSTALLMENTS.ToString(),
                    Type = TransactionsType.CREDIT_CARD.ToString()

                };
                await _transactionsService.InsertToCard(cardBill.Card.CardId, transactionRequest);
            }
            else
            {
                cardBill.Pay(request.FullPayment, request.AmountPaid, 0);
            }

            await _repository.UpdatePay(cardBillId, cardBill);
            return SuccessResponse(CardBill, CardBillPay);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    #endregion

    #region [ Private Methods ]

    private Status ReturnStatusBill(DateTime closingDate, DateTime expirationDate, Status status)
    {
        var result = Status.OPEN;
        if (closingDate < DateTime.UtcNow) result = Status.CLOSED;
        else if (closingDate > DateTime.UtcNow) result = Status.OPEN;
        else if (closingDate < DateTime.UtcNow && status != Status.PAID && status != Status.PARTIALLY_PAID && expirationDate <= DateTime.UtcNow) result = Status.AVAILABLE_FOR_PAYMENT;
        else if (expirationDate > DateTime.UtcNow && status != Status.PAID && status != Status.PARTIALLY_PAID) result = Status.OVERDUE;

        return result;
    }

    private CardBillResponse ReturnData(CardBillModel model, List<TransactionsModel> transactions)
    {
        var result = new CardBillResponse
        {
            CardBillId = model.CardBillId,
            CardId = model.Card.CardId,
            ExpirationDate = model.ExpirationDate,
            Status = model.Status.GetEnumDescription(),
            TotalValue = model.TotalValue,
        };

        result.Transactions = transactions.Select(t => new CardBillTransactionsResponse
        {
            TransactionId = t.TransactionId,
            Name = t.Name,
            DatePurchase = t.DatePurchase,
            Value = t.Value ?? t.Repetition.ValueInstallment,
            CurrentInstallment = t.Repetition != null ? t.Repetition.CurrentInstallment : 1,
            NumberInstallments = t.Repetition != null ? t.Repetition.NumberInstallments : 1
        })
        .OrderBy(t => t.DatePurchase)
        .ToList();

        return result;
    }

    #endregion
}