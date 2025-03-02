using FinanceControl.Domain.Entities;
using System;
using System.Collections.Generic;

namespace FinanceControl.Application.Extensions.Utils.Repetition
{
    public class AddRepetition : IAddRepetition
    {
        public List<TransactionsModel> AddRepetitionCard(int quantityInstallment, int currentInstallment, int closingDay, int expirationDay, TransactionsModel model)
        {
            var iteration = 0;
            var closingDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, closingDay);
            var dateExpiration = new DateTime(DateTime.Now.Year, DateTime.Now.Month, expirationDay);

            var transactions = new List<TransactionsModel>();
            if (model.DatePurchase < closingDate)
            {
                while (currentInstallment <= quantityInstallment)
                {
                    transactions.Add(ReturnModel(model, dateExpiration, iteration, currentInstallment++));
                    iteration++;
                }
            }
            else
            {
                iteration = 1;
                while (currentInstallment <= quantityInstallment)
                {
                    transactions.Add(ReturnModel(model, dateExpiration, iteration, currentInstallment++));
                    iteration++;
                }
            }

            return transactions;
        }

        public List<TransactionsModel> AddRepetitionCardDebitAndPix(int quantityInstallment, int currentInstallment, TransactionsModel model)
        {
            var iteration = 0;
            var installment = model.Installment;
            var dateExpiration = DateTime.UtcNow;

            var transactions = new List<TransactionsModel>();
            if (!installment)
            {
                transactions.Add(ReturnModel(model, DateTime.UtcNow, iteration, currentInstallment));
            }
            if (DateTime.Today <= dateExpiration)
            {
                while (currentInstallment <= quantityInstallment)
                {
                    transactions.Add(ReturnModel(model, dateExpiration, iteration, currentInstallment++));
                    iteration++;
                }
            }
            else
            {
                iteration = 1;
                while (currentInstallment <= quantityInstallment)
                {
                    transactions.Add(ReturnModel(model, dateExpiration, iteration, currentInstallment++));
                    iteration++;
                }
            }

            return transactions;
        }

        public List<TransactionsModel> AddRepetitionBankSlip(int quantityInstallment, int currentInstallment, int expirationDay, TransactionsModel model)
        {
            var iteration = 0;
            var installment = model.Installment;
            var dateExpiration = new DateTime(DateTime.Now.Year, DateTime.Now.Month, expirationDay);

            var transactions = new List<TransactionsModel>();
            if(DateTime.Today <= dateExpiration)
            {
                while (currentInstallment <= quantityInstallment)
                {
                    transactions.Add(ReturnModel(model, dateExpiration, iteration, currentInstallment++));
                    iteration++;
                }
            }
            else
            {
                iteration = 1;
                while (currentInstallment <= quantityInstallment)
                {
                    transactions.Add(ReturnModel(model, dateExpiration, iteration, currentInstallment++));
                    iteration++;
                }
            }

            return transactions;
        }

        private TransactionsModel ReturnModel(TransactionsModel model, DateTime dateExpiration, int iteration, int currentInstallment)
        {
            return new TransactionsModel
            {
                TransactionId = model.TransactionId,
                Name = model.Name,
                Active = model.Active,
                CashFlow = model.CashFlow,
                CreatedBy = model.CreatedBy,
                CreationDate = model.CreationDate,
                DatePurchase = model.DatePurchase,
                ExpenseType = model.ExpenseType,
                Installment = model.Installment,
                PaymentDetails = model.PaymentDetails,
                Assigned = model.Assigned,
                Type = model.Type,
                Value = model.Value,
                ExpirationDate = dateExpiration.AddMonths(iteration),
                YearMonthReference = dateExpiration.AddMonths(iteration).ToString("yyyy/MM"),
                Repetition = model.Installment ? new RepetitionModel(model.Repetition.NumberInstallments, currentInstallment, model.Repetition.ValueInstallment) : null
            };
        }
    }
}
