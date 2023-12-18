using FinanceControl.Cards.DTO_s;
using FinanceControl.Extensions.Paginated;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.BaseService;
using FinanceControl.Application.Extensions.Enum;
using FinanceControl.Application.Services.Cards.DTO_s;
using FinanceControl.Extensions.AppSettings;
using ILogger = Serilog.ILogger;
using FinanceControl.Application.Services.Cards.Model.Enum;
using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Application.Services.Cards.Repository;
using FinanceControl.Application.Services.Wallet.Repository;

namespace FinanceControl.Application.Services.Cards.Service;

public class CardService : BaseService
{
    #region [ Constructor ]
    public CardService(IAppSettings appSettings, ILogger logger,
        Guid currentUserId) : base(logger: logger, appSettings: appSettings,
        currentUserId: currentUserId)
    {
    }
    #endregion

    #region [ Messages ]

    private const string WalletNotFound = "Carteira não encontrada";
    private const string CardNotFound = "Cartão não encontrado";
    private const string Card = "Cartão";

    #endregion

    #region [ Public Methods ]

    /// <summary>
    /// Serviço para inserir um Cartão
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> InsertCard(CardInsertRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (request.WalletId == Guid.Empty || userId == Guid.Empty || request == null)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var walletRepository = new WalletRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
            var wallet = await walletRepository.GetById(request.WalletId, userId);
            if (wallet == null)
                return ErrorResponse(WalletNotFound);

            var date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, request.ExpirationDay);
            var closingDay = date.AddDays(-6);
            var model = new CardModel(userId, request.Name, request.Color, request.ExpirationDay, closingDay.Day, Enum.Parse<CardType>(request.Type), new CardWalletModel(wallet.WalletId, wallet.Name));

            using var repository = new CardRepository(_appSettings.GetMongoDb(), _logger);

            await repository.InsertOneAsync(model);

            return SuccessResponse(Card, Message.SUCCESSFULLY_ADDED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Obter um cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public async Task<ResultValue> GetById(Guid walletId, Guid cardId)
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

    /// <summary>
    /// Serviço para Obter todos os cartões
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="search"></param>
    /// <param name="take"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public async Task<ResultValue> GetAll(Guid walletId, string search, int take, int skip)
    {
        try
        {
            if (walletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new CardRepository(_appSettings.GetMongoDb(), _logger);

            var list = await repository.GetAll(walletId, search, take, skip);
            if (list == null)
                return SuccessResponse(new PaginatedResponse<CardResponse> { Records = new List<CardResponse>() });

            var record = _mapper.Map<List<CardResponse>>(list.Records);
            var result = new PaginatedResponse<CardResponse>
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
    /// Serviço para atualizar os dados de um Cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> Update(Guid cardId, CardUpdateRequest request)
    {
        try
        {
            if (request == null || cardId == Guid.Empty || request.WalletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new CardRepository(_appSettings.GetMongoDb(), _logger);

            var model = await repository.GetById(cardId, request.WalletId);
            if (model == null)
                return ErrorResponse(CardNotFound);

            model.Update(request.Name, request.Color, request.ExpirationDay, request.ClosingDay, Enum.Parse<CardType>(request.Type));
            await repository.Update(request.WalletId, model);

            return SuccessResponse(Card, Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Ativar um Cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public async Task<ResultValue> Active(Guid walletId, Guid cardId)
    {
        try
        {
            if (cardId == Guid.Empty || walletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new CardRepository(_appSettings.GetMongoDb(), _logger);

            await repository.UpdateActive(walletId, cardId);

            return SuccessResponse(Card, Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Inativar um Cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public async Task<ResultValue> Inactive(Guid walletId, Guid cardId)
    {
        try
        {
            if (cardId == Guid.Empty || walletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new CardRepository(_appSettings.GetMongoDb(), _logger);

            await repository.UpdateInactive(walletId, cardId);

            return SuccessResponse(Card, Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Excluir um cartão
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public async Task<ResultValue> Delete(Guid cardId, Guid walletId)
    {
        try
        {
            if (cardId == Guid.Empty || walletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new CardRepository(_appSettings.GetMongoDb(), _logger);
            await repository.Delete(walletId, cardId);

            return SuccessResponse(Card, Message.SUCCESSFULLY_DELETED.GetEnumDescription());
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