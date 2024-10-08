using Amazon.Runtime.Internal;
using FinanceControl.Application.Extensions.BaseService;
using FinanceControl.Application.Extensions.Enum;
using FinanceControl.Application.Services.CardBill.DTO_s.Request;
using FinanceControl.Application.Services.CardBill.Model;
using FinanceControl.Application.Services.CardBill.Repository;
using FinanceControl.Application.Services.Cards.DTO_s;
using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Application.Services.Cards.Model.Enum;
using FinanceControl.Application.Services.Cards.Repository;
using FinanceControl.Application.Services.Transactions.Model;
using FinanceControl.Application.Services.Transactions.Model.Enum;
using FinanceControl.Application.Services.Transactions.Repository;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Cards.DTO_s;
using FinanceControl.Extensions.AppSettings;
using FinanceControl.Extensions.Paginated;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.CardBill.Service;

public class CardBillService : BaseService
{
    #region [ Constructor ]
    public CardBillService(IAppSettings appSettings, ILogger logger,
        Guid currentUserId) : base(logger: logger, appSettings: appSettings,
        currentUserId: currentUserId)
    {
    }
    #endregion

    #region [ Messages ]

    private const string WalletNotFound = "Carteira não encontrada";
    private const string CardNotFound = "Cartão de Crédito não encontrado";
    private const string CardBill = "Fatura";
    private const string CardBillNotFound = "Não foi encontrado dados para gerar Fatura do mês selecionado!";

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
    public async Task<ResultValue> GenerateBill(Guid walletId, Guid cardId, int month, int year)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (walletId == Guid.Empty || cardId == Guid.Empty || month == 0 || year == 0)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var cardRepository = new CardRepository(_appSettings.GetMongoDb(), _logger);
            var cardCredit = await cardRepository.GetById(cardId, walletId);
            if (cardCredit == null)
                return ErrorResponse(CardNotFound);

            var expirationDate = new DateTime(year, month, cardCredit.ExpirationDay);
            var closingDate = new DateTime(year, month, cardCredit.ClosingDay);

            using var transactionRepository = new TransactionsRepository(_appSettings.GetMongoDb(), _logger);
            var transactions = await transactionRepository.GetAllByCardIdAndDate(cardId, closingDate.AddMonths(-1), closingDate);
            if (transactions == null)
                return ErrorResponse(CardBillNotFound);

            var model = new CardBillModel(userId, expirationDate, new CardBillCardCredit(cardCredit.CardId, cardCredit.Name));

            model.Status = ReturnStatusBill(closingDate, expirationDate, model.Status);
            model.TotalValue = transactions.Sum(t => t.Value.Value);

            await new CardBillRepository(_appSettings.GetMongoDb(), _logger).InsertOneAsync(model);

            var result = ReturnData(model, transactions);
            return SuccessResponse(result, CardBill, Message.SUCCESSFULLY_ADDED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Obter uma Fatura de um mês por Cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    public async Task<ResultValue> GetById(Guid cardId, int month)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (cardId == Guid.Empty || userId == Guid.Empty || walletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new CardRepository(_appSettings.GetMongoDb(), _logger);

            var record = await repository.GetById(cardId, walletId);
            if (record == null)
                return ErrorResponse(CardNotFound);

            var result = _mapper.Map<CardResponse>(record);

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    #endregion

    #region [ Private Methods ]

    public Status ReturnStatusBill(DateTime closingDate, DateTime expirationDate, Status status)
    {
        var result = Status.OPEN;
        if (closingDate < DateTime.UtcNow) result = Status.CLOSED;
        else if (closingDate > DateTime.UtcNow) result = Status.OPEN;
        else if (closingDate < DateTime.UtcNow && status != Status.PAID && expirationDate <= DateTime.UtcNow) result = Status.AVAILABLE_FOR_PAYMENT;
        else if (expirationDate > DateTime.UtcNow && status != Status.PAID) result = Status.OVERDUE;

        return result;
    }

    public CardBillResponse ReturnData(CardBillModel model, List<TransactionsModel> transactions)
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
            Value = t.Value.Value,
            CurrentInstallment = t.Repetition.CurrentInstallment,
            NumberInstallments = t.Repetition.NumberInstallments
        })
        .OrderBy(t => t.DatePurchase)
        .ToList();

        return result;
    }

    #endregion
}