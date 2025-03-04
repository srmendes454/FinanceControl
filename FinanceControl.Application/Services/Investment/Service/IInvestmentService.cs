using FinanceControl.Application.Services.Investment.DTO_s.Request;
using FinanceControl.Infra.BaseService;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Investment.Service
{
    public interface IInvestmentService
    {
        Task<ResultValue> Insert(InvestmentInsertRequest request);
        Task<ResultValue> GetById(Guid investmentId);
        Task<ResultValue> GetAll(Guid walletId, string search, int take, int skip);
        Task<ResultValue> Update(Guid investmentId, InvestmentInsertRequest request);
        Task<ResultValue> Delete(Guid investmentId);
        ResultValue ListInvestmentTypes();
    }
}
