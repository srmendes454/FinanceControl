using FinanceControl.Domain.Entities;
using FinanceControl.Infra.BaseRepository;
using System.Threading.Tasks;
using System;
using FinanceControl.Application.Extensions.Paginated;
using System.Collections.Generic;

namespace FinanceControl.Application.Services.Division.Repository
{
    public interface IDivisionRepository : IBaseRepository<DivisionModel>
    {
        Task<DivisionModel> GetById(Guid divisionId);
        Task<PaginatedResponse<DivisionModel>> GetAll(Guid walletId, string search, int take, int skip);
        Task Update(DivisionModel model);
        Task UpdateLimit(Guid divisionId, List<LimitModel> limits);
        Task<DivisionModel> GetLimitsById(Guid divisionId);
        Task Delete(Guid divisionId);
    }
}
