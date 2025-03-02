using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Application.Services.Pix.DTO_s.Request;
using FinanceControl.Application.Services.Pix.DTO_s.Response;
using FinanceControl.Application.Services.Pix.Repository;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Pix.Service
{
    public class PixService : BaseService<PixService>, IPixService
    {
        #region [ Fields ]

        private readonly IPixRepository _repository;

        #endregion

        #region [ Constructor ]

        public PixService(IAppSettings appSettings, IPixRepository repository) : base(appSettings)
        {
            _repository = repository;
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

                using var walletRepository = new WalletRepository(_appSettings.GetMongoDb(), _appSettings);
                var wallet = await walletRepository.GetById(request.WalletId, userId);
                if (wallet == null)
                    return ErrorResponse(WalletNotFound);

                var model = new PixModel(userId, request.Name, request.LinkedAccount, Enum.Parse<PixType>(request.Type), request.Color, new PixWalletModel(wallet.WalletId, wallet.Name));

                await _repository.InsertOneAsync(model);

                return SuccessResponse(Pix, Message.SUCCESSFULLY_ADDED_M.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter um Pix
        /// </summary>
        /// <param name="pixId"></param>
        /// <returns></returns>
        public async Task<ResultValue> GetById(Guid pixId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (pixId == Guid.Empty || userId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var record = await _repository.GetById(pixId);
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

                var list = await _repository.GetAll(walletId, search, take, skip);
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
                if (request == null || pixId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var model = await _repository.GetById(pixId);
                if (model == null)
                    return ErrorResponse(PixNotFound);

                model.Update(request.Name, request.LinkedAccount, Enum.Parse<PixType>(request.Type), request.Color);
                await _repository.Update(model);

                return SuccessResponse(Pix, Message.SUCCESSFULLY_UPDATED_M.GetEnumDescription());
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
        public async Task<ResultValue> Delete(Guid pixId)
        {
            try
            {
                if (pixId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                await _repository.Delete(pixId);

                return SuccessResponse(Pix, Message.SUCCESSFULLY_DELETED_M.GetEnumDescription());
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
