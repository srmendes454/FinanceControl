using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Application.Services.BankSlip.DTO_s.Request;
using FinanceControl.Application.Services.BankSlip.DTO_s.Response;
using FinanceControl.Application.Services.BankSlip.Repository;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.BankSlip.Service
{
    public class BankSlipService : BaseService<BankSlipService>, IBankSlipService
    {
        #region [ Fields ]

        private readonly IBankSlipRepository _repository;
        private readonly IWalletRepository _walletRepository;

        #endregion

        #region [ Constructor ]

        public BankSlipService(IAppSettings appSettings, IBankSlipRepository repository, IWalletRepository walletRepository) : base(appSettings)
        {
            _repository = repository;
            _walletRepository = walletRepository;
        }

        #endregion

        #region [ Messages ]

        private const string WalletNotFound = "Carteira não encontrada";
        private const string BankSlipNotFound = "Boleto não encontrado";
        private const string BankSlip = "Boleto";

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Serviço para inserir um Boleto
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Insert(BankSlipInsertRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (request.WalletId == Guid.Empty || userId == Guid.Empty || request == null)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var wallet = await _walletRepository.GetById(request.WalletId, userId);
                if (wallet == null)
                    return ErrorResponse(WalletNotFound);

                var model = new BankSlipModel(userId, request.Name, request.ExpirationDay, new BankSlipWalletModel(wallet.WalletId, wallet.Name));

                await _repository.InsertOneAsync(model);

                return SuccessResponse(BankSlip, Message.SUCCESSFULLY_ADDED_M.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter um Boleto
        /// </summary>
        /// <param name="bankSlipId"></param>
        /// <returns></returns>
        public async Task<ResultValue> GetById(Guid bankSlipId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (bankSlipId == Guid.Empty || userId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var record = await _repository.GetById(bankSlipId);
                if (record == null)
                    return ErrorResponse(BankSlipNotFound);

                var result = _mapper.Map<BankSlipResponse>(record);

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter todos os Boletos
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
                    return SuccessResponse(new PaginatedResponse<BankSlipResponse> { Records = new List<BankSlipResponse>() });

                var record = _mapper.Map<List<BankSlipResponse>>(list.Records);
                var result = new PaginatedResponse<BankSlipResponse>
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
        /// Serviço para atualizar os dados de um Boleto
        /// </summary>
        /// <param name="bankSlipId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Update(Guid bankSlipId, BankSlipUpdateRequest request)
        {
            try
            {
                if (request == null || bankSlipId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var model = await _repository.GetById(bankSlipId);
                if (model == null)
                    return ErrorResponse(BankSlipNotFound);

                model.Update(request.Name, request.ExpirationDay);
                await _repository.Update(model);

                return SuccessResponse(BankSlip, Message.SUCCESSFULLY_UPDATED_M.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Excluir um Boleto
        /// </summary>
        /// <param name="bankSlipId"></param>
        /// <returns></returns>
        public async Task<ResultValue> Delete(Guid bankSlipId)
        {
            try
            {
                if (bankSlipId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                await _repository.Delete(bankSlipId);

                return SuccessResponse(BankSlip, Message.SUCCESSFULLY_DELETED_M.GetEnumDescription());
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
