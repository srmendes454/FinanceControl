using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Application.Services.Cards.DTO_s;
using FinanceControl.Application.Services.Division.DTO_s.Request;
using FinanceControl.Application.Services.Division.DTO_s.Response;
using FinanceControl.Application.Services.Division.Repository;
using FinanceControl.Application.Services.Transactions.Repository;
using FinanceControl.Application.Services.Wallet.Repository;
using FinanceControl.Cards.DTO_s;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;
using FinanceControl.Infra.AppSettings;
using FinanceControl.Infra.BaseService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FinanceControl.Application.Services.Division.Service
{
    public class DivisionService : BaseService<DivisionService>, IDivisionService
    {
        #region [ Fields ]

        private readonly IDivisionRepository _repository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionsRepository _transactionRepository;

        #endregion

        #region [ Constructor ]

        public DivisionService(IAppSettings appSettings, IDivisionRepository repository, IWalletRepository walletRepository, ITransactionsRepository transactionRepository) : base(appSettings)
        {
            _repository = repository;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }

        #endregion

        #region [ Messages ]

        private const string WalletNotFound = "Carteira não encontrada";
        private const string DivisionNotFound = "Repartição não encontrada";
        private const string Division = "Repartição";
        private const string Limit = "Limite";

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Serviço para Inserir uma Repartição/Divisão
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Insert(DivisionInsertRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (request.WalletId == Guid.Empty || userId == Guid.Empty || request == null)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var wallet = await _walletRepository.GetById(request.WalletId, userId);
                if (wallet == null)
                    return ErrorResponse(WalletNotFound);

                var model = new DivisionModel(request.Name, request.Color, request.Percent, wallet.WalletId, wallet.Name);

                await _repository.InsertOneAsync(model);

                return SuccessResponse(Division, Message.SUCCESSFULLY_ADDED_F.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter uma Repartição
        /// </summary>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        public async Task<ResultValue> GetById(Guid divisionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (divisionId == Guid.Empty || userId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var division = await _repository.GetById(divisionId);
                if (division == null)
                    return ErrorResponse(DivisionNotFound);

                var result = _mapper.Map<DivisionResponse>(division);

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Obter todas as Repartições
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<ResultValue> GetAll(Guid walletId, string search, int year, int month, int take, int skip)
        {
            try
            {
                if (walletId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var list = await _repository.GetAll(walletId, search, take, skip);
                if (list == null)
                    return SuccessResponse(new PaginatedResponse<DivisionAllResponse> { Records = [] });

                var yearMonthReference = $"{year:0000}/{month:00}";
                var salary = await _transactionRepository.GetTransactionSalary(yearMonthReference);

                var transactions = new List<TransactionsModel>();
                var limits = list.Records.SelectMany(x => x.Limits ?? []).ToList();
                if (limits.Count > 0)
                {
                    var expensesType = limits.Select(l => l.Name).ToList();
                    transactions = await _transactionRepository.GetTransactionByExpenseType(yearMonthReference, expensesType);
                }

                var transactionsGroup = transactions.GroupBy(x => x.ExpenseType).ToList();
                var sum = transactionsGroup.Select(x => new
                {
                    ExpenseType = x.Key,
                    TotalValue = x.Sum(t => t.Value ?? t.Repetition.ValueInstallment),
                });

                var record = list.Records.Select(d =>
                {
                    var limitValue = Math.Round(salary * (d.Percent / 100), 2);
                    var limits = d.Limits?.Select(l =>
                    {
                        var value = Math.Round(limitValue * (l.Percent / 100), 2);
                        var valueUsed = sum?.FirstOrDefault(x => x.ExpenseType.Equals(l.Name))?.TotalValue ?? 0;

                        return new LimitResponse
                        {
                            LimitId = l.LimitId,
                            Name = l.Name.GetEnumDescription(),
                            Percent = l.Percent,
                            PercentUsed = Math.Round((valueUsed / value) * 100, 2),
                            Value = value,
                            ValueUsed = valueUsed
                        };
                    }).ToList();

                    return new DivisionAllResponse
                    {
                        DivisionId = d.DivisionId,
                        Name = d.Name,
                        Color = d.Color,
                        Percent = d.Percent,
                        LimitValue = limitValue,
                        Limits = limits ?? []
                    };
                });

                var result = new PaginatedResponse<DivisionAllResponse>
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
        /// Serviço para atualizar os dados de uma Repartição
        /// </summary>
        /// <param name="divisionId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> Update(Guid divisionId, DivisionInsertRequest request)
        {
            try
            {
                if (request == null || divisionId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var model = await _repository.GetById(divisionId);
                if (model == null)
                    return ErrorResponse(DivisionNotFound);

                model.Update(request.Name, request.Color, request.Percent);
                await _repository.Update(model);

                return SuccessResponse(Division, Message.SUCCESSFULLY_UPDATED_F.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Serviço para Excluir uma Repartição
        /// </summary>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        public async Task<ResultValue> Delete(Guid divisionId)
        {
            try
            {
                if (divisionId == Guid.Empty)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                await _repository.Delete(divisionId);

                return SuccessResponse(Division, Message.SUCCESSFULLY_DELETED_F.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #region [ Limits ]

        /// <summary>
        /// Serviço para Salvar Limite para uma repartição
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultValue> SaveLimit(Guid divisionId, List<LimitInsertRequest> request)
        {
            try
            {
                if (divisionId == Guid.Empty || request == null)
                    return ErrorResponse(Message.INVALID_OBJECT.GetEnumDescription());

                var division = await _repository.GetLimitsById(divisionId);
                if (division == null)
                    return ErrorResponse(DivisionNotFound);

                division.Limits = [];
                foreach (var item in request)
                {
                    var limit = new LimitModel(Enum.Parse<ExpenseType>(item.Name), item.Percent);
                    division.Limits.Add(limit);
                }

                await _repository.UpdateLimit(divisionId, division.Limits);

                return SuccessResponse(Limit, Message.SUCCESSFULLY_ADDED_M.GetEnumDescription());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion

        #endregion
    }
}
