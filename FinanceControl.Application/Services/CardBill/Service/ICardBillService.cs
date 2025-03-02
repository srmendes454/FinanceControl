using FinanceControl.Application.Services.CardBill.DTO_s.Request;
using FinanceControl.Infra.BaseService;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.CardBill.Service
{
    public interface ICardBillService
    {
        Task<ResultValue> GenerateCardBill(Guid cardId, int month, int year);
        Task<ResultValue> UpdateCardBill(Guid cardId, Guid cardBillId, int month, int year);
        Task<ResultValue> PayCardBill(Guid cardBillId, PayCardBillRequest request);
    }
}
