using FinanceControl.Application.Services.Transactions.Model.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.Serialization;
using FinanceControl.Application.Extensions.BaseModel;

namespace FinanceControl.Application.Services.Transactions.Model
{
    [DataContract]
    [Table("Transaction")]
    public class TransactionsModel : EntityBase
    {
        #region [ Constructor ]
        public TransactionsModel()
        {
            TransactionId = Guid.NewGuid();
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
        [BsonRepresentation(BsonType.String)]
        public TransactionsCashFlow CashFlow { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public TransactionsType Type { get; set; }

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
    }

    public class RepetitionModel
    {
        #region [ Constructor ]
        public RepetitionModel()
        {
            RepetitionId = Guid.NewGuid();
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
        public int QuantityInstallment { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public int CurrentInstallment { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public double ValueInstallment { get; set; }
        #endregion
    }

    public class PaymentDetailsModel
    {
        #region [ Constructor ]
        public PaymentDetailsModel()
        {
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

        [DataMember]
        [BsonIgnoreIfNull]
        public string Color { get; set; }
        #endregion
    }

    public class AssignedModel
    {
        #region [ Constructor ]

        public AssignedModel()
        {
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

        #endregion
    }
}
