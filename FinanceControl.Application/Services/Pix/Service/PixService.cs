using FinanceControl.Application.Extensions.BaseService;
using FinanceControl.Application.Extensions.Enum;
using FinanceControl.Application.Services.Cards.Model.Enum;
using FinanceControl.Application.Services.Pix.DTO_s.Request;
using FinanceControl.Application.Services.Pix.DTO_s.Response;
using FinanceControl.Application.Services.Pix.Model;
using FinanceControl.Application.Services.Pix.Model.Enum;
using FinanceControl.Application.Services.Pix.Repository;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Extensions.AppSettings;
using FinanceControl.Extensions.Paginated;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Pix.Service
{
    public class PixService : BaseService
    {
        #region [ Constructor ]

        public PixService(IAppSettings appSettings, ILogger logger, Guid currentUserId) : base(appSettings, logger, currentUserId)
        {
        }

        #endregion

        #region [ Messages ]

        private const string WalletNotFound = "Carteira não encontrada";
        private const string PixNotFound = "Pix não encontrado";
        private const string Pix = "Pix";

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Serviço para inserir um Pix
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Insert(PixInsertRequest request)
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

                var model = new PixModel(userId, request.Name, request.LinkedAccount, request.ExpirationDay, Enum.Parse<PixType>(request.Type), request.Color, new PixWalletModel(wallet.WalletId, wallet.Name));

                await new PixRepository(_appSettings.GetMongoDb(), _logger).InsertOneAsync(model);

                return SuccessResponse(Pix, Message.SUCCESSFULLY_ADDED.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter um Pix
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="pixId"></param>
        /// <returns></returns>
        public async Task<ResultValue> GetById(Guid walletId, Guid pixId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (pixId == Guid.Empty || userId == Guid.Empty || walletId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                using var repository = new PixRepository(_appSettings.GetMongoDb(), _logger);

                var record = await repository.GetById(pixId, walletId);
                if (record == null)
                    return ErrorResponse(PixNotFound);

                var result = _mapper.Map<PixResponse>(record);

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter todos os Pixs
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

                using var repository = new PixRepository(_appSettings.GetMongoDb(), _logger);

                var list = await repository.GetAll(walletId, search, take, skip);
                if (list == null)
                    return SuccessResponse(new PaginatedResponse<PixResponse> { Records = new List<PixResponse>() });

                var record = _mapper.Map<List<PixResponse>>(list.Records);
                var result = new PaginatedResponse<PixResponse>
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
        /// Serviço para atualizar os dados de um Pix
        /// </summary>
        /// <param name="pixId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Update(Guid pixId, PixInsertRequest request)
        {
            try
            {
                if (request == null || pixId == Guid.Empty || request.WalletId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                using var repository = new PixRepository(_appSettings.GetMongoDb(), _logger);
                var model = await repository.GetById(pixId, request.WalletId);
                if (model == null)
                    return ErrorResponse(PixNotFound);

                model.Update(request.Name, request.LinkedAccount, request.ExpirationDay, Enum.Parse<PixType>(request.Type), request.Color);
                repository.Update(request.WalletId, model);

                return SuccessResponse(Pix, Message.SUCCESSFULLY_UPDATED.GetEnumDescription());
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
        /// <param name="pixId"></param>
        /// <returns></returns>
        public async Task<ResultValue> Delete(Guid pixId, Guid walletId)
        {
            try
            {
                if (pixId == Guid.Empty || walletId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                new PixRepository(_appSettings.GetMongoDb(), _logger).Delete(walletId, pixId);

                return SuccessResponse(Pix, Message.SUCCESSFULLY_DELETED.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion

        #region [ List Enuns ]

        /// <summary>
        /// Listagem do Tipos de Chaves Pix
        /// </summary>
        /// <returns></returns>
        public ResultValue ListPixTypes()
        {
            try
            {
                var result = Enum.GetValues<PixType>().GetEnumDescriptionAtributte();
                if (result == null || result.Count <= 0)
                    return ErrorResponse("Tipos de Chave Pix não encontrada!");

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
