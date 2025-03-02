using FinanceControl.Domain.Entities;
using FinanceControl.Infra.BaseRepository;
using System.Threading.Tasks;
using System;
using FinanceControl.Application.Extensions.Paginated;

namespace FinanceControl.Application.Services.BankSlip.Repository
{
    public interface IBankSlipRepository : IBaseRepository<BankSlipModel>
    {
        Task<BankSlipModel> GetById(Guid bankSlipId);
        Task<PaginatedResponse<BankSlipModel>> GetAll(Guid walletId, string search, int take, int skip);
        Task Update(BankSlipModel model);
        Task Delete(Guid bankSlipId);
    }
}
