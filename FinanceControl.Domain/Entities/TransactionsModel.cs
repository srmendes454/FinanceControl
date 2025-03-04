using FinanceControl.Domain.Enuns;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FinanceControl.Domain.Entities
{
    [DataContract]
    [Table("Transaction")]
    public class TransactionsModel : EntityBase
    {
        #region [ Constructor ]

        public TransactionsModel() { }
        public TransactionsModel(Guid userId, string name, DateTime datePurchase, bool installment, TransactionsCashFlow cashFlow, TransactionsType type, ExpenseType expenseType)
        {
            TransactionId = Guid.NewGuid();
            Name = name;
            DatePurchase = datePurchase;
            Installment = installment;
            CashFlow = cashFlow;
            Type = type;
            ExpenseType = expenseType;
            Active = true;
            CreationDate = DateTime.Now;
            CreatedBy = userId;
        }
        #endregion

        #region [ Properties ] 
        [DataMember]
        [BsonIgnoreIfNull]
        [BsonElement("TransactionId")]
        [BsonRepresentation(BsonType.String)]
        public Guid TransactionId { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public DateTime DatePurchase { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public DateTime ExpirationDate { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string YearMonthReference { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public TransactionsCashFlow CashFlow { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public TransactionsType Type { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public ExpenseType ExpenseType { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public bool Installment { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public double? Value { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public RepetitionModel Repetition { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public PaymentDetailsModel PaymentDetails { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public AssignedModel Assigned { get; set; }
        #endregion

        #region [ Public Methods ]

        public void Update(string name, DateTime datePurchase, bool installment, TransactionsCashFlow cashFlow, ExpenseType expenseType)
        {
            Name = name;
            DatePurchase = datePurchase;
            Installment = installment;
            CashFlow = cashFlow;
            ExpenseType = expenseType;
            UpdateDate = DateTime.Now;
        }

        public void LoadData(double value, DateTime dateExpiration)
        {
            Value = value;
            ExpirationDate = dateExpiration;
            YearMonthReference = dateExpiration.ToString("yyyy/MM");
        }

        public void LoadData(double value, DateTime dateExpiration, int closingDay)
        {
            if (DatePurchase.Day < closingDay)
            {
                Value = value;
                ExpirationDate = dateExpiration;
                YearMonthReference = ExpirationDate.ToString("yyyy/MM");
            }
            else
            {
                Value = value;
                ExpirationDate = dateExpiration.AddMonths(1);
                YearMonthReference = ExpirationDate.ToString("yyyy/MM");
            }
        }

        public TransactionsModel CopyFull(DateTime dateExpiration, int iteration = 0, int currentInstallment = 0)
        {
            var result = new TransactionsModel
            {
                TransactionId = TransactionId,
                Name = Name,
                Active = Active,
                CashFlow = CashFlow,
                CreatedBy = CreatedBy,
                CreationDate = CreationDate,
                DatePurchase = DatePurchase,
                ExpenseType = ExpenseType,
                Installment = Installment,
                PaymentDetails = PaymentDetails,
                Assigned = Assigned,
                Type = Type,
                Value = Value,
                ExpirationDate = dateExpiration.AddMonths(iteration),
                YearMonthReference = dateExpiration.AddMonths(iteration).ToString("yyyy/MM"),
                Repetition = Installment ? new RepetitionModel(Repetition.NumberInstallments, currentInstallment, Repetition.ValueInstallment) : null
            };

            return result;
        }

        public List<TransactionsModel> AddRepetitionCard(int quantityInstallment, int currentInstallment, int closingDay, int expirationDay)
        {
            var iteration = 0;
            var dateExpiration = new DateTime(DatePurchase.Year, DatePurchase.Month, expirationDay);

            var transactions = new List<TransactionsModel>();
            if (DatePurchase.Day < closingDay)
            {
                while (currentInstallment <= quantityInstallment)
                {
                    transactions.Add(CopyFull(dateExpiration, iteration, currentInstallment++));
                    iteration++;
                }
            }
            else
            {
                iteration = 1;
                while (currentInstallment <= quantityInstallment)
                {
                    transactions.Add(CopyFull(dateExpiration, iteration, currentInstallment++));
                    iteration++;
                }
            }

            return transactions;
        }

        public List<TransactionsModel> AddRepetitionBankSlip(int quantityInstallment, int currentInstallment, int expirationDay)
        {
            var iteration = 0;
            var dateExpiration = new DateTime(DatePurchase.Year, DatePurchase.Month, expirationDay);

            var transactions = new List<TransactionsModel>();
            if (DatePurchase.Day <= expirationDay)
            {
                while (currentInstallment <= quantityInstallment)
                {
                    transactions.Add(CopyFull(dateExpiration, iteration, currentInstallment++));
                    iteration++;
                }
            }
            else
            {
                iteration = 1;
                while (currentInstallment <= quantityInstallment)
                {
                    transactions.Add(CopyFull(dateExpiration, iteration, currentInstallment++));
                    iteration++;
                }
            }

            return transactions;
        }

        #endregion
    }

    public class RepetitionModel
    {
        #region [ Constructor ]
        public RepetitionModel(int numberInstallments, int currentInstallment, double valueInstallment)
        {
            RepetitionId = Guid.NewGuid();
            NumberInstallments = numberInstallments;
            CurrentInstallment = currentInstallment;
            ValueInstallment = valueInstallment;
        }
        #endregion

        #region [ Properties ]
        [DataMember]
        [BsonIgnoreIfNull]
        [BsonElement("RepetitionId")]
        [BsonRepresentation(BsonType.String)]
        public Guid RepetitionId { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public int NumberInstallments { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public int CurrentInstallment { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public double ValueInstallment { get; set; }
        #endregion

        #region [ Public Methods ]

        public void Update(int numberInstallments, int currentInstallment, double valueInstallment)
        {
            NumberInstallments = numberInstallments;
            CurrentInstallment = currentInstallment;
            ValueInstallment = valueInstallment;
        }

        #endregion
    }

    public class PaymentDetailsModel
    {
        #region [ Constructor ]
        public PaymentDetailsModel(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        #endregion

        #region [ Properties ]
        [DataMember]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Name { get; set; }
        #endregion
    }

    public class AssignedModel
    {
        #region [ Constructor ]

        public AssignedModel(Guid userId, string name, string email)
        {
            AssignedId = userId;
            Name = name;
            Email = email;
        }

        #endregion

        #region [ Properties ]
        [DataMember]
        [BsonIgnoreIfNull]
        [BsonElement("AssignedId")]
        [BsonRepresentation(BsonType.String)]
        public Guid AssignedId { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Email { get; set; }

        #endregion

        #region [ Public Methods ]

        public void Update(Guid assignedId, string name, string email)
        {
            AssignedId = assignedId;
            Name = name;
            Email = email;
        }

        #endregion
    }
}
