using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Application.Services.Transactions.Model;
using System.Collections.Generic;

namespace FinanceControl.Application.Extensions.Utils.Repetition
{
    public interface IAddRepetition
    {
        List<TransactionsModel> AddRepetitionCard(int quantityInstallment, int currentInstallment, int closingDay, int expirationDay, TransactionsModel model);
        List<TransactionsModel> AddRepetitionCardDebitAndPix(int quantityInstallment, int currentInstallment, int expirationDay, TransactionsModel model);
        List<TransactionsModel> AddRepetitionBankSlip(int quantityInstallment, int currentInstallment, int expirationDay, TransactionsModel model);
    }
}
