using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Application.Services.AccountBank.Repository;
using FinanceControl.Application.Services.AccountBank.DTO_s.Request;
using FinanceControl.Application.Services.AccountBank.DTO_s.Response;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.AccountBank.Service
{
    public class AccountBankService : BaseService<AccountBankService>, IAccountBankService
    {
        #region [ Fields ]

        private readonly IAccountBankRepository _repository;

        #endregion

        #region [ Constructor ]

        public AccountBankService(IAppSettings appSettings, IAccountBankRepository repository) : base(appSettings)
        {
            _repository = repository;
        }

        #endregion

        #region [ Messages ]

        private const string WalletNotFound = "Carteira não encontrada";
        private const string AccountBankNotFound = "Conta Bancária não encontrada";
        private const string AccountBank = "Conta Bancária";

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Serviço para inserir uma Conta Bancária
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Insert(AccountBankInsertRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (request.WalletId == Guid.Empty || userId == Guid.Empty || request == null)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                using var walletRepository = new WalletRepository(_appSettings.GetMongoDb(), _appSettings);
                var wallet = await walletRepository.GetById(request.WalletId, userId);
                if (wallet == null)
                    return ErrorResponse(WalletNotFound);

                var model = new AccountBankModel(userId, request.Name, Enum.Parse<AccountBankType>(request.Type), request.Color, new AccountBankWalletModel(wallet.WalletId, wallet.Name));

                await _repository.InsertOneAsync(model);

                return SuccessResponse(AccountBank, Message.SUCCESSFULLY_ADDED_F.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter uma Conta Bancária
        /// </summary>
        /// <param name="accountBankId"></param>
        /// <returns></returns>
        public async Task<ResultValue> GetById(Guid accountBankId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (accountBankId == Guid.Empty || userId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var record = await _repository.GetById(accountBankId);
                if (record == null)
                    return ErrorResponse(AccountBankNotFound);

                var result = _mapper.Map<AccountBankResponse>(record);

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter todas as Contas Bancárias
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
                    return SuccessResponse(new PaginatedResponse<AccountBankResponse> { Records = new List<AccountBankResponse>() });

                var record = _mapper.Map<List<AccountBankResponse>>(list.Records);
                var result = new PaginatedResponse<AccountBankResponse>
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
        /// Serviço para atualizar os dados de uma Conta Bancária
        /// </summary>
        /// <param name="accountBankId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Update(Guid accountBankId, AccountBankInsertRequest request)
        {
            try
            {
                if (request == null || accountBankId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var model = await _repository.GetById(accountBankId);
                if (model == null)
                    return ErrorResponse(AccountBankNotFound);

                model.Update(request.Name, Enum.Parse<AccountBankType>(request.Type), request.Color);
                await _repository.Update(model);

                return SuccessResponse(AccountBank, Message.SUCCESSFULLY_UPDATED_F.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Excluir uma Conta Bancária
        /// </summary>
        /// <param name="accountBankId"></param>
        /// <returns></returns>
        public async Task<ResultValue> Delete(Guid accountBankId)
        {
            try
            {
                if (accountBankId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                await _repository.Delete(accountBankId);

                return SuccessResponse(AccountBank, Message.SUCCESSFULLY_DELETED_M.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion

        #region [ List Enuns ]

        /// <summary>
        /// Listagem do Tipos de Contas Bancárias
        /// </summary>
        /// <returns></returns>
        public ResultValue ListAccountBankTypes()
        {
            try
            {
                var result = Enum.GetValues<AccountBankType>().GetEnumDescriptionAtributte();
                if (result == null || result.Count <= 0)
                    return ErrorResponse("Tipos de Conta Bancária não encontradas!");

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
}
