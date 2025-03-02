using FinanceControl.Application.Services.Cards.DTO_s;
using FinanceControl.Infra.BaseService;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Cards.Service
{
    public interface ICardService
    {
        Task<ResultValue> InsertCard(CardInsertRequest request);
        Task<ResultValue> GetById(Guid cardId);
        Task<ResultValue> GetAll(Guid walletId, string search, int take, int skip);
        Task<ResultValue> Update(Guid cardId, CardUpdateRequest request);
        Task<ResultValue> Active(Guid cardId);
        Task<ResultValue> Inactive(Guid cardId);
        Task<ResultValue> Delete(Guid cardId);
        ResultValue ListCardTypes();
    }
}
