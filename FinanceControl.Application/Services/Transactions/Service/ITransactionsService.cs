using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Infra.BaseService;
using System;
using System.Threading.Tasks;

namespace FinanceControl.Application.Services.Transactions.Service
{
    public interface ITransactionsService
    {
        Task<ResultValue> InsertToCard(Guid cardId, TransactionsInsertRequest request);
        Task<ResultValue> InsertToBankSlip(Guid bankSlipId, TransactionsInsertRequest request);
        Task<ResultValue> InsertToAccountBank(Guid accountBankId, TransactionsInsertRequest request);
        Task<ResultValue> InsertToInvestment(Guid investmentId, InvestedAmountRequest request);
        Task<ResultValue> GetAllByPaymentId(Guid paymentId, Guid assignedId, string search, string type, int year, int month, int take, int skip);
        Task<ResultValue> GetByIdAndDate(Guid transactionId, int year, int month);
        Task<ResultValue> Update(Guid transactionId, int year, int month, TransactionsUpdateRequest request);
        Task<ResultValue> Delete(Guid transactionId, int year, int month, bool deleteAll);
        Task<ResultValue> MoveTransaction(Guid transactionId, bool next);
        Task<ResultValue> RedeemInvestedAmount(Guid investmentId, RedeemInvestedAmountRequest request);
        ResultValue ListExpenseType();
        ResultValue ListCashFlow();
        ResultValue ListTransactionsType();
        Task<ResultValue> ListAssignedTransactions(string search, int take, int skip);
        Task<ResultValue> EvaluateAssignedTransaction(TransactionsEvaluateAssignedRequest request);
    }
}
