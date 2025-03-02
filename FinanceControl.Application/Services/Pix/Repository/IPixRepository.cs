using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Domain.Entities;
using FinanceControl.Infra.BaseRepository;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Pix.Repository
{
    public interface IPixRepository : IBaseRepository<PixModel>
    {
        Task<PixModel> GetById(Guid pixId);
        Task<PaginatedResponse<PixModel>> GetAll(Guid walletId, string search, int take, int skip);
        Task Update(PixModel model);
        Task Delete(Guid pixId);
    }
}
