using FinanceControl.Domain.Entities;
using FinanceControl.Infra.BaseRepository;
using System.Threading.Tasks;
using System;

namespace FinanceControl.Application.Services.CardBill.Repository
{
    public interface ICardBillRepository : IBaseRepository<CardBillModel>
    {
        Task<CardBillModel> GetById(Guid cardBillId);
        Task Update(Guid cardBillId, CardBillModel model);
        Task UpdatePay(Guid cardBillId, CardBillModel model);
    }
}
