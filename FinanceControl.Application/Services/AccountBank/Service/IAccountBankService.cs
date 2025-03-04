using FinanceControl.Application.Services.AccountBank.DTO_s.Request;
using FinanceControl.Infra.BaseService;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.AccountBank.Service
{
    public interface IAccountBankService
    {
        Task<ResultValue> Insert(AccountBankInsertRequest request);
        Task<ResultValue> GetById(Guid accountBankId);
        Task<ResultValue> GetAll(Guid walletId, string search, int take, int skip);
        Task<ResultValue> Update(Guid accountBankId, AccountBankInsertRequest request);
        Task<ResultValue> Delete(Guid accountBankId);
        ResultValue ListAccountBankTypes();
    }
}
