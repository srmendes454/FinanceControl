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

    private const string WalletNotFound = "WALLET_NOT_FOUND";
    private const string CardNotFound = "CARD_NOT_FOUND";

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
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            var userId = GetCurrentUserId();
            if (request.WalletId == Guid.Empty && userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var walletRepository = new WalletRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
            var wallet = await walletRepository.GetById(request.WalletId, userId);
            if (wallet == null)
                return ErrorResponse(WalletNotFound);

            var model = new CardModel
            {
                Name = request.Name,
                Color = request.Color,
                Number = request.Number,
                DateExpiration = request.DateExpiration,
                AvailableLimit = request.AvailableLimit,
                CardType = Enum.Parse<CardType>(request.CardType),
                Status = Status.TO_WIN,
                Wallet = new CardWalletModel
                {
                    WalletId = wallet.WalletId,
                    Name = wallet.Name
                },
                CreatedBy = GetCurrentUserId(),
                CreationDate = DateTime.Now,
                Active = true
            };

            using var repository = new CardRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            await repository.InsertOneAsync(model);

            return SuccessResponse("CARD", Message.SUCCESSFULLY_ADDED.ToString());
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
            if (cardId == Guid.Empty && userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new CardRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var record = await repository.GetById(cardId, walletId);
            if (record == null)
                return ErrorResponse(CardNotFound);

            var result = _mapper.Map<CardResponse>(record);
            var d = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, record.DateExpiration);
            result.ClosingDay = d.AddDays(-7);

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
            using var repository = new CardRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

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
    public async Task<ResultValue> Update(Guid cardId, CardInsertRequest request)
    {
        try
        {
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            if (cardId == Guid.Empty && request.WalletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new CardRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var model = await repository.GetById(cardId, request.WalletId);
            if (model == null)
                return ErrorResponse(CardNotFound);

            model.Name = request.Name;
            model.Number = request.Number;
            model.Color = request.Color;
            model.DateExpiration = request.DateExpiration;
            model.AvailableLimit = request.AvailableLimit;
            model.CardType = Enum.Parse<CardType>(request.CardType);

            await repository.Update(request.WalletId, model);

            return SuccessResponse("CARD", Message.SUCCESSFULLY_UPDATED.ToString());
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
            if (cardId == Guid.Empty && walletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new CardRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            await repository.UpdateActive(walletId, cardId);

            return SuccessResponse("CARD", Message.SUCCESSFULLY_UPDATED.ToString());
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
            if (cardId == Guid.Empty && walletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new CardRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            await repository.UpdateInactive(walletId, cardId);

            return SuccessResponse("CARD", Message.SUCCESSFULLY_UPDATED.ToString());
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
            if (cardId == Guid.Empty && walletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.ToString());

            using var repository = new CardRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());
            var filter = Builders<CardModel>.Filter
                .Where(x => x.CardId.Equals(cardId)
                            && x.Wallet.WalletId.Equals(walletId));

            await repository.DeleteOneAsync(filter);

            return SuccessResponse("CARD", Message.SUCCESSFULLY_DELETED.ToString());
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