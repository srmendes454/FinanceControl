using FinanceControl.Application.Services.Division.DTO_s.Request;
using FinanceControl.Infra.BaseService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Division.Service
{
    public interface IDivisionService
    {
        Task<ResultValue> Insert(DivisionInsertRequest request);
        Task<ResultValue> GetById(Guid divisionId);
        Task<ResultValue> GetAll(Guid walletId, string search, int year, int month, int take, int skip);
        Task<ResultValue> Update(Guid divisionId, DivisionInsertRequest request);
        Task<ResultValue> Delete(Guid divisionId);
        Task<ResultValue> SaveLimit(Guid divisionId, List<LimitInsertRequest> request);
    }
}
