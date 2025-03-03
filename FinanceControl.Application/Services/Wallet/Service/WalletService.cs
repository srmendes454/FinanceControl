using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Application.Services.Transactions.Repository;
using FinanceControl.Application.Services.User.Repository;
using FinanceControl.Application.Services.Wallet.DTO_s.Request;
using FinanceControl.Application.Services.Wallet.DTO_s.Response;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseService;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Wallet.Service;

public class WalletService : BaseService<WalletService>, IWalletService
{
    #region [ Fields ]

    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _repository;

    #endregion

    #region [ Constructor ]

    public WalletService(IAppSettings appSettings, IUserRepository useRepository, IWalletRepository repository) : base(appSettings)
    {
        _userRepository = useRepository;
        _repository = repository;
    }

    #endregion

    #region [ Messages ]

    private const string Wallet = "Carteira";
    private const string WalletNotFound = "Carteira não encontrada";
    private const string OptimizeIncomeNotFound = "Repartição não encontrada";
    private const string OptimizeIncome = "Repartição";

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
            if (request == null || userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var user = await _userRepository.GetById(userId);
            if (user == null)
                return ErrorResponse(Message.USER_NOT_FOUND.GetEnumDescription());

            var optimizeIncome = GenerateMethod20_30_50();
            var model = new WalletModel(request.Name, request.Color, userId, user.Name, optimizeIncome);

            await _repository.InsertOneAsync(model);

            return SuccessResponse(Wallet, Message.SUCCESSFULLY_ADDED_F.GetEnumDescription());
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
            if (walletId == Guid.Empty || userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var record = await _repository.GetById(walletId, userId);
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
            if (userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var list = await _repository.GetAllByUser(userId);
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

            var model = await _repository.GetById(walletId, userId);
            if (model == null)
                return ErrorResponse(WalletNotFound);

            model.Update(request.Name, request.Color);

            await _repository.Update(walletId, model);

            return SuccessResponse(Wallet, Message.SUCCESSFULLY_UPDATED_F.GetEnumDescription());
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
            if (walletId == Guid.Empty || userId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            await _repository.Delete(walletId, userId);

            return SuccessResponse(Wallet, Message.SUCCESSFULLY_DELETED_F.GetEnumDescription());
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

            await _repository.UpdateActive(walletId, userId);

            return SuccessResponse(Wallet, Message.SUCCESSFULLY_UPDATED_F.GetEnumDescription());
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

            await _repository.UpdateInactive(walletId, userId);

            return SuccessResponse(Wallet, Message.SUCCESSFULLY_UPDATED_F.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    #region [ Optimize Income ]

    /// <summary>
    /// Serviço para Obter todas as Divisões de Renda por Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <returns></returns>
    public async Task<ResultValue> GetAllOptimizeIncome(Guid walletId)
    {
        try
        {
            if (walletId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var list = await _repository.GetAllByWallet(walletId);
            if (list == null)
                return ErrorResponse(Message.LIST_EMPTY.GetEnumDescription());

            var result = _mapper.Map<List<OptimizeIncomeResponse>>(list);

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para Obtem uma Divisão da Renda por Carteira
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="optimizeIncomeId"></param>
    /// <returns></returns>
    public async Task<ResultValue> GetOptimizeIncomeById(Guid walletId, Guid optimizeIncomeId)
    {
        try
        {
            if (walletId == Guid.Empty && optimizeIncomeId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var record = await _repository.GetOptimizeIncomeById(walletId, optimizeIncomeId);
            if (record == null)
                return ErrorResponse(OptimizeIncomeNotFound);

            var result = _mapper.Map<OptimizeIncomeResponse>(record);

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para atualiza uma Divisão de Renda
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="optimizeIncomeId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ResultValue> UpdateOptimizeIncome(Guid walletId, Guid optimizeIncomeId, OptimizeIncomeRequest request)
    {
        try
        {
            if (walletId == Guid.Empty || optimizeIncomeId == Guid.Empty || request.Equals(null))
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var list = await _repository.GetAllByWallet(walletId);
            if (list == null)
                return ErrorResponse(Message.LIST_EMPTY.GetEnumDescription());

            list.Where(o => o.OptimizeIncomeId.Equals(optimizeIncomeId)).ToList().ForEach(x =>
            {
                x.Update(request.Name, request.Color, request.Percent);
            });

            await _repository.UpdateOptimizeIncome(walletId, list);

            return SuccessResponse(OptimizeIncome, Message.SUCCESSFULLY_UPDATED_F.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    /// <summary>
    /// Serviço para deletar uma Divisão de Renda
    /// </summary>
    /// <param name="walletId"></param>
    /// <param name="optimizeIncomeId"></param>
    /// <returns></returns>
    public async Task<ResultValue> DeleteOptimizeIncome(Guid walletId, Guid optimizeIncomeId)
    {
        try
        {
            if (walletId == Guid.Empty || optimizeIncomeId == Guid.Empty)
                return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

            var list = await _repository.GetAllByWallet(walletId);
            if (list == null)
                return ErrorResponse(Message.LIST_EMPTY.GetEnumDescription());

            var optimizeIncome = list.FirstOrDefault(x => x.OptimizeIncomeId.Equals(optimizeIncomeId));
            if (optimizeIncome == null)
                return ErrorResponse(OptimizeIncomeNotFound);

            list.RemoveAll(x => x.OptimizeIncomeId.Equals(optimizeIncomeId));
            await _repository.UpdateOptimizeIncome(walletId, list);

            return SuccessResponse(OptimizeIncome, Message.SUCCESSFULLY_ADDED_F.GetEnumDescription());
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    #endregion

    #endregion

    #region [ Private Methods ]

    private static List<OptimizeIncomeModel> GenerateMethod20_30_50()
    {
        return new List<OptimizeIncomeModel>
        {
            new("Essencial", "#1875FF", 50),
            new("Lazer", "#04C300", 30),
            new("Investimento", "#F39200", 20)
        };
    }

    #endregion
}