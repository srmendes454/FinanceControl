using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Application.Extensions.Utils.Email;
using FinanceControl.Application.Services.Cards.DTO_s;
using FinanceControl.Application.Services.Cards.Repository;
using FinanceControl.Application.Services.User.Repository;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Cards.DTO_s;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseService;
using FinanceControl.Infra.RequestContainer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Cards.Service;

public class CardService : BaseService<CardService>, ICardService
{
    #region [ Fields ]

    private readonly ICardRepository _repository;
    private readonly IWalletRepository _walletRepository;

    #endregion

    #region [ Constructor ]
    public CardService(IAppSettings appSettings, ICardRepository repository, IWalletRepository walletRepository) : base(appSettings)
    {
        _repository = repository;
        _walletRepository = walletRepository;
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

            var wallet = await _walletRepository.GetById(request.WalletId, userId);
            if (wallet == null)
                return ErrorResponse(WalletNotFound);

            var closingDay = 0;
            if (request.Type != CardType.DEBIT.ToString())
            {
                var date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, request.ExpirationDay);
                closingDay = date.AddDays(-6).Day;
            }

            var model = new CardModel(userId, request.Name, request.Color, request.ExpirationDay, closingDay, Enum.Parse<CardType>(request.Type), new CardWalletModel(wallet.WalletId, wallet.Name));

            await _repository.InsertOneAsync(model);

            return SuccessResponse(Card, Message.SUCCESSFULLY_ADDED_M.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Obter um cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public async Task<ResultValue> GetById(Guid cardId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (cardId == Guid.Empty || userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var record = await _repository.GetById(cardId);
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

            var list = await _repository.GetAll(walletId, search, take, skip);
            if (list == null)
                return SuccessResponse(new PaginatedResponse<CardResponse> { Records = new List<CardResponse>() });

            var record = _mapper.Map<List<CardResponse>>(list.Records);
            var result = new PaginatedResponse<CardResponse>
            {
                Records = [..record],
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
            if (request == null || cardId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var model = await _repository.GetById(cardId);
            if (model == null)
                return ErrorResponse(CardNotFound);

            model.Update(request.Name, request.Color, request.ExpirationDay, request.ClosingDay, Enum.Parse<CardType>(request.Type));
            await _repository.Update(model);

            return SuccessResponse(Card, Message.SUCCESSFULLY_UPDATED_M.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Ativar um Cartão
    /// </summary>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public async Task<ResultValue> Active(Guid cardId)
    {
        try
        {
            if (cardId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            await _repository.UpdateActive(cardId);

            return SuccessResponse(Card, Message.SUCCESSFULLY_UPDATED_M.GetEnumDescription());
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
    public async Task<ResultValue> Inactive(Guid cardId)
    {
        try
        {
            if (cardId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            await _repository.UpdateInactive(cardId);

            return SuccessResponse(Card, Message.SUCCESSFULLY_UPDATED_M.GetEnumDescription());
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
    public async Task<ResultValue> Delete(Guid cardId)
    {
        try
        {
            if (cardId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            await _repository.Delete(cardId);

            return SuccessResponse(Card, Message.SUCCESSFULLY_DELETED_M.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    #endregion

    #region [ List Enuns ]

    /// <summary>
    /// Listagem do Tipos de Cartões
    /// </summary>
    /// <returns></returns>
    public ResultValue ListCardTypes()
    {
        try
        {
            var result = Enum.GetValues<CardType>().GetEnumDescriptionAtributte();
            if (result == null || result.Count <= 0)
                return ErrorResponse("Tipos de Cartões não encontrado!");

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