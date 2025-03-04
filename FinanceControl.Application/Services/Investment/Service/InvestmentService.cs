using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Application.Services.Investment.DTO_s.Request;
using FinanceControl.Application.Services.Investment.DTO_s.Response;
using FinanceControl.Application.Services.Investment.Repository;
using FinanceControl.Application.Services.Transactions.Repository;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Investment.Service
{
    public class InvestmentService : BaseService<InvestmentService>, IInvestmentService
    {
        #region [ Fields ]

        private readonly IInvestmentRepository _repository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionsRepository _transactionRepository;

        #endregion

        #region [ Constructor ]

        public InvestmentService(IAppSettings appSettings, IInvestmentRepository repository, IWalletRepository walletRepository, ITransactionsRepository transactionsRepository) : base(appSettings)
        {
            _repository = repository;
            _walletRepository = walletRepository;
            _transactionRepository = transactionsRepository;
        }

        #endregion

        #region [ Messages ]

        private const string WalletNotFound = "Carteira não encontrada";
        private const string InvestmentNotFound = "Investimento não encontrado";
        private const string Investment = "Investimento";

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Serviço para Inserir um Investimento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Insert(InvestmentInsertRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (request.WalletId == Guid.Empty || userId == Guid.Empty || request == null)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var wallet = await _walletRepository.GetById(request.WalletId, userId);
                if (wallet == null)
                    return ErrorResponse(WalletNotFound);

                var model = new InvestmentModel(userId, request.Name, request.Color, request.MonthlyProfitability, Enum.Parse<InvestmentType>(request.Type), wallet.WalletId, wallet.Name);

                await _repository.InsertOneAsync(model);

                return SuccessResponse(Investment, Message.SUCCESSFULLY_ADDED_M.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter um Investimento por Id
        /// </summary>
        /// <param name="investmentId"></param>
        /// <returns></returns>
        public async Task<ResultValue> GetById(Guid investmentId)
        {
            try
            {
                if (investmentId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var record = await _repository.GetById(investmentId);
                if (record == null)
                    return ErrorResponse(InvestmentNotFound);

                var result = _mapper.Map<InvestmentResponse>(record);

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter todos os Serviços da Carteira Paginado e Filtrado
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
                    return SuccessResponse(new PaginatedResponse<InvestmentAllResponse> { Records = new List<InvestmentAllResponse>() });

                var investmentIds = list.Records.Select(x => x.InvestmentId).ToList();
                var types = new List<TransactionsType> { TransactionsType.INVESTMENT };
                var transactions = await _transactionRepository.GetTransactionByPaymentIds(investmentIds, types);

                var transactionsGroup = transactions.GroupBy(x => x.PaymentDetails.Id).ToList();
                var sum = transactionsGroup.Select(x => new
                {
                    PaymentId = x.Key,
                    TotalEntryValue = x.Where(t => t.CashFlow == TransactionsCashFlow.ENTRY).Sum(t => t.Value ?? t.Repetition.ValueInstallment),
                    TotalExitValue = x.Where(t => t.CashFlow == TransactionsCashFlow.EXIT).Sum(t => t.Value ?? t.Repetition.ValueInstallment),
                });

                var record = list.Records.Select(i =>
                {
                    var totalEntryValue = sum?.FirstOrDefault(x => x.PaymentId.Equals(i.InvestmentId))?.TotalEntryValue ?? 0;
                    var totalExitValue = sum?.FirstOrDefault(x => x.PaymentId.Equals(i.InvestmentId))?.TotalExitValue ?? 0;
                    var amountInvested = Math.Round(totalEntryValue - totalExitValue, 2);
                    return new InvestmentAllResponse
                    {
                        InvestmentId = i.InvestmentId,
                        Name = i.Name,
                        Color = i.Color,
                        Type = i.Type.GetEnumDescription(),
                        AmountInvested = Math.Round(amountInvested + (amountInvested * (i.MonthlyProfitability / 100)), 2)
                    };
                });

                var result = new PaginatedResponse<InvestmentAllResponse>
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
        /// Serviço para Atualizar um Investimento
        /// </summary>
        /// <param name="investmentId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Update(Guid investmentId, InvestmentInsertRequest request)
        {
            try
            {
                if (request == null || investmentId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var model = await _repository.GetById(investmentId);
                if (model == null)
                    return ErrorResponse(InvestmentNotFound);

                model.Update(request.Name, request.Color, request.MonthlyProfitability, Enum.Parse<InvestmentType>(request.Type));
                await _repository.Update(model);

                return SuccessResponse(Investment, Message.SUCCESSFULLY_UPDATED_M.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Deletar um Investimento
        /// </summary>
        /// <param name="investmentId"></param>
        /// <returns></returns>
        public async Task<ResultValue> Delete(Guid investmentId)
        {
            try
            {
                if (investmentId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                await _repository.Delete(investmentId);

                return SuccessResponse(Investment, Message.SUCCESSFULLY_DELETED_M.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Listar os Tipos de Investimentos
        /// </summary>
        /// <returns></returns>
        public ResultValue ListInvestmentTypes()
        {
            try
            {
                var result = Enum.GetValues<InvestmentType>().GetEnumDescriptionAtributte();
                if (result == null || result.Count <= 0)
                    return ErrorResponse("Tipos de Investimentos não encontrados!");

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion
    }
}
