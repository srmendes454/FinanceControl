using FinanceControl.Application.Services.User.Repository;
using FinanceControl.Application.Services.Wallet.Model;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Extensions.AppSettings;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceControl.Application.Extensions.BaseService;
using FinanceControl.Application.Extensions.Enum;
using FinanceControl.Application.Services.Wallet.DTO_s.Request;
using FinanceControl.Application.Services.Wallet.DTO_s.Response;

namespace FinanceControl.Application.Services.Wallet.Service;

public class WalletService : BaseService
{
    #region [ Constructor ]

    public WalletService(IAppSettings appSettings, ILogger logger,
        Guid currentUserId) : base(logger: logger, appSettings: appSettings,
        currentUserId: currentUserId)
    {

    }

    #endregion

    #region [ Messages ]

    private const string Wallet = "Carteira";
    private const string WalletNotFound = "Carteira não encontrada";

    #endregion

    #region [ Public Methods ]

    /// <summary>
    /// Serviço para inserir uma Carteira
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> Insert(WalletInsertRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (request == null)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var userRepository = new UserRepository(_appSettings.GetMongoDb(), _logger);

            var user = await userRepository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var model = new WalletModel(request.Name, request.Color, userId, user.Name);

            using var repository = new WalletRepository(_appSettings.GetMongoDb(), _logger);

            await repository.InsertOneAsync(model);

            return SuccessResponse(Wallet, Message.SUCCESSFULLY_ADDED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Obter a Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <returns></returns>
    public async Task<ResultValue> GetById(Guid walletId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (walletId == Guid.Empty && userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new WalletRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var record = await repository.GetById(walletId, userId);
            if (record == null)
                return ErrorResponse(WalletNotFound);

            var result = _mapper.Map<WalletResponse>(record);

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Obter todas as Carteiras do Usuário
    /// </summary>
    /// <returns></returns>
    public async Task<ResultValue> GetAll()
    {
        try
        {
            var userId = GetCurrentUserId();
            using var repository = new WalletRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var list = await repository.GetAllByUser(userId);
            if (list == null)
                return ErrorResponse(Message.LIST_EMPTY.GetEnumDescription());

            var result = _mapper.Map<List<WalletResponse>>(list);

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para atualizar os dados de uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> Update(Guid walletId, WalletInsertRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (walletId == Guid.Empty || userId == Guid.Empty || request.Equals(null))
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new WalletRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            var model = await repository.GetById(walletId, userId);
            if (model == null)
                return ErrorResponse(WalletNotFound);

            model.Update(request.Name, request.Color);

            await repository.Update(walletId, model);

            return SuccessResponse(Wallet, Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Excluir uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <returns></returns>
    public async Task<ResultValue> Delete(Guid walletId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (walletId == Guid.Empty && userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new WalletRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            await repository.Delete(walletId, userId);

            return SuccessResponse(Wallet, Message.SUCCESSFULLY_DELETED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Ativar uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <returns></returns>
    public async Task<ResultValue> Active(Guid walletId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (walletId == Guid.Empty && userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new WalletRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            await repository.UpdateActive(walletId, userId);

            return SuccessResponse(Wallet, Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Inativar uma Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <returns></returns>
    public async Task<ResultValue> Inactive(Guid walletId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (walletId == Guid.Empty && userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            using var repository = new WalletRepository(logger: _logger, mongoDb: _appSettings.GetMongoDb());

            await repository.UpdateInactive(walletId, userId);

            return SuccessResponse(Wallet, Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
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