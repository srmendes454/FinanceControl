using FinanceControl.Application.Services.Wallet.DTO_s.Request;
using FinanceControl.Infra.BaseService;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Wallet.Service
{
    public interface IWalletService
    {
        Task<ResultValue> Insert(WalletInsertRequest request);
        Task<ResultValue> GetById(Guid walletId);
        Task<ResultValue> GetAll();
        Task<ResultValue> Update(Guid walletId, WalletInsertRequest request);
        Task<ResultValue> Delete(Guid walletId);
        Task<ResultValue> Active(Guid walletId);
        Task<ResultValue> Inactive(Guid walletId);
        Task<ResultValue> GetAllOptimizeIncome(Guid walletId);
        Task<ResultValue> GetOptimizeIncomeById(Guid walletId, Guid optimizeIncomeId);
        Task<ResultValue> UpdateOptimizeIncome(Guid walletId, Guid optimizeIncomeId, OptimizeIncomeRequest request);
        Task<ResultValue> DeleteOptimizeIncome(Guid walletId, Guid optimizeIncomeId);
    }
}
