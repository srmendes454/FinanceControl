using FinanceControl.Application.Extensions.BaseService;
using FinanceControl.Application.Extensions.Enum;
using FinanceControl.Application.Services.BankSlip.DTO_s.Request;
using FinanceControl.Application.Services.BankSlip.DTO_s.Response;
using FinanceControl.Application.Services.BankSlip.Model;
using FinanceControl.Application.Services.BankSlip.Repository;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Extensions.AppSettings;
using FinanceControl.Extensions.Paginated;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.BankSlip.Service
{
    public class BankSlipService : BaseService
    {
        #region [ Constructor ]

        public BankSlipService(IAppSettings appSettings, ILogger logger, Guid currentUserId) : base(appSettings, logger, currentUserId)
        {
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

                using var walletRepository = new WalletRepository(_appSettings.GetMongoDb(), _logger);
                var wallet = await walletRepository.GetById(request.WalletId, userId);
                if (wallet == null)
                    return ErrorResponse(WalletNotFound);

                var model = new BankSlipModel(userId, request.Name, request.ExpirationDay, new BankSlipWalletModel(wallet.WalletId, wallet.Name));

                await new BankSlipRepository(_appSettings.GetMongoDb(), _logger).InsertOneAsync(model);

                return SuccessResponse(BankSlip, Message.SUCCESSFULLY_ADDED.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter um Boleto
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="bankSlipId"></param>
        /// <returns></returns>
        public async Task<ResultValue> GetById(Guid walletId, Guid bankSlipId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (bankSlipId == Guid.Empty || userId == Guid.Empty || walletId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                using var repository = new BankSlipRepository(_appSettings.GetMongoDb(), _logger);

                var record = await repository.GetById(bankSlipId, walletId);
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

                using var repository = new BankSlipRepository(_appSettings.GetMongoDb(), _logger);

                var list = await repository.GetAll(walletId, search, take, skip);
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
                if (request == null || bankSlipId == Guid.Empty || request.WalletId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                using var repository = new BankSlipRepository(_appSettings.GetMongoDb(), _logger);
                var model = await repository.GetById(bankSlipId, request.WalletId);
                if (model == null)
                    return ErrorResponse(BankSlipNotFound);

                model.Update(request.Name, request.ExpirationDay);
                repository.Update(request.WalletId, model);

                return SuccessResponse(BankSlip, Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Excluir um Boleto
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="bankSlipId"></param>
        /// <returns></returns>
        public async Task<ResultValue> Delete(Guid bankSlipId, Guid walletId)
        {
            try
            {
                if (bankSlipId == Guid.Empty || walletId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                new BankSlipRepository(_appSettings.GetMongoDb(), _logger).Delete(walletId, bankSlipId);

                return SuccessResponse(BankSlip, Message.SUCCESSFULLY_DELETED.GetEnumDescription());
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
