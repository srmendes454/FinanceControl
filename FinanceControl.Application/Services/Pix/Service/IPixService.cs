using FinanceControl.Application.Services.Pix.DTO_s.Request;
using FinanceControl.Infra.BaseService;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Pix.Service
{
    public interface IPixService
    {
        Task<ResultValue> Insert(PixInsertRequest request);
        Task<ResultValue> GetById(Guid pixId);
        Task<ResultValue> GetAll(Guid walletId, string search, int take, int skip);
        Task<ResultValue> Update(Guid pixId, PixInsertRequest request);
        Task<ResultValue> Delete(Guid pixId);
        ResultValue ListPixTypes();
    }
}
