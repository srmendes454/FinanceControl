using FinanceControl.Domain.Entities;
using System.Threading.Tasks;
using System;
using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Infra.BaseRepository;

namespace FinanceControl.Application.Services.Cards.Repository
{
    public interface ICardRepository : IBaseRepository<CardModel>
    {
        Task<CardModel> GetById(Guid cardId);
        Task<PaginatedResponse<CardModel>> GetAll(Guid walletId, string search, int take, int skip);
        Task Update(CardModel model);
        Task UpdateActive(Guid cardId);
        Task UpdateInactive(Guid cardId);
        Task Delete(Guid cardId);
    }
}
