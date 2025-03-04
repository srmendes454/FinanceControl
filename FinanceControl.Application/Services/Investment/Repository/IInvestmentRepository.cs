using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Domain.Entities;
using FinanceControl.Infra.BaseRepository;
using System.Threading.Tasks;
using System;

namespace FinanceControl.Application.Services.Investment.Repository
{
    public interface IInvestmentRepository : IBaseRepository<InvestmentModel>
    {
        Task<InvestmentModel> GetById(Guid investmentId);
        Task<PaginatedResponse<InvestmentModel>> GetAll(Guid walletId, string search, int take, int skip);
        Task Update(InvestmentModel model);
        Task Delete(Guid investmentId);
    }
}
