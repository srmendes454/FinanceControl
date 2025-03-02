using FinanceControl.Application.Services.BankSlip.DTO_s.Request;
using FinanceControl.Infra.BaseService;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.BankSlip.Service
{
    public interface IBankSlipService
    {
        Task<ResultValue> Insert(BankSlipInsertRequest request);
        Task<ResultValue> GetById(Guid bankSlipId);
        Task<ResultValue> GetAll(Guid walletId, string search, int take, int skip);
        Task<ResultValue> Update(Guid bankSlipId, BankSlipUpdateRequest request);
        Task<ResultValue> Delete(Guid bankSlipId);
    }
}
