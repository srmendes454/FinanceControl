using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Domain.Entities;
using FinanceControl.Infra.BaseRepository;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.AccountBank.Repository
{
    public interface IAccountBankRepository : IBaseRepository<AccountBankModel>
    {
        Task<AccountBankModel> GetById(Guid accountBankId);
        Task<PaginatedResponse<AccountBankModel>> GetAll(Guid walletId, string search, int take, int skip);
        Task Update(AccountBankModel model);
        Task Delete(Guid accountBankId);
    }
}
