using FinanceControl.Application.Extensions.Paginated;
using FinanceControl.Domain.Entities;
using FinanceControl.Infra.BaseRepository;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using FinanceControl.Domain.Enuns;

namespace FinanceControl.Application.Services.Transactions.Repository
{
    public interface ITransactionsRepository : IBaseRepository<TransactionsModel>
    {
        Task<PaginatedResponse<TransactionsModel>> GetAllByPaymentId(Guid paymentId, Guid assignedId, string search, string type, int year, int month, int take, int skip);
        Task<TransactionsModel> GetById(Guid transactionId);
        Task<double> GetTransactionSalary(string yearMonthReference);
        Task<List<TransactionsModel>> GetTransactionByExpenseType(string yearMonthReference, List<ExpenseType> expensesType);
        Task<TransactionsModel> GetByIdAndDate(Guid transactionId, int year, int month);
        Task<List<TransactionsModel>> GetAllByCardIdAndDate(Guid cardId, int year, int month);
        Task<List<TransactionsModel>> GetTransactionsById(Guid transactionId);
        Task Update(Guid transactionId, TransactionsModel model);
        Task Delete(Guid transactionId, int year, int month);
        Task DeleteTransactions(Guid transactionId);
        Task UpdateAssigned(Guid transactionId, TransactionsModel model);
        Task UpdateAllAssigned(Guid transactionId, AssignedModel assignedModel);
        Task UpdateMove(Guid transactionId, string yearMonthReference, TransactionsModel transaction);
        Task<PaginatedResponse<TransactionsModel>> ListAssignedTransactions(Guid userId, string search, int take, int skip);
    }
}
