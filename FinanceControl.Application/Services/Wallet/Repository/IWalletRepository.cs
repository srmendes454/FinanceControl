using FinanceControl.Domain.Entities;
using FinanceControl.Infra.BaseRepository;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace FinanceControl.Application.Services.Wallet.Repository
{
    public interface IWalletRepository : IBaseRepository<WalletModel>
    {
        Task<WalletModel> GetById(Guid walletId, Guid userId);
        Task<List<WalletModel>> GetAllByUser(Guid userId);
        Task Update(Guid walletId, WalletModel model);
        Task UpdateActive(Guid walletId, Guid userId);
        Task UpdateInactive(Guid walletId, Guid userId);
        Task Delete(Guid walletId, Guid userId);
        Task<List<OptimizeIncomeModel>> GetAllByWallet(Guid walletId);
        Task<OptimizeIncomeModel> GetOptimizeIncomeById(Guid walletId, Guid optimizeIncomeId);
        Task UpdateOptimizeIncome(Guid walletId, List<OptimizeIncomeModel> optimizeIncome);
    }
}
