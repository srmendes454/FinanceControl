using FinanceControl.Domain.Enuns;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FinanceControl.Domain.Entities
{
    [DataContract]
    [Table("Division")]
    public class DivisionModel : EntityBase
    {
        #region [ Constructor ]

        public DivisionModel(string name, string color, double percent, Guid walletId, string nameWallet)
        {
            DivisionId = Guid.NewGuid();
            Active = true;
            Name = name;
            Color = color;
            Percent = percent;
            Wallet = new DivisionWalletModel(walletId, nameWallet);
        }

        public DivisionModel()
        {
            
        }

        #endregion

        #region [ Properties ]

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonElement("DivisionId")]
        [BsonRepresentation(BsonType.String)]
        public Guid DivisionId { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Color { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public double Percent { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public DivisionWalletModel Wallet { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public List<LimitModel> Limits { get; set; }

        #endregion

        #region [ Public Methods ]

        public void Update(string name, string color, double percent)
        {
            Name = name;
            Color = color;
            Percent = percent;
        }

        #endregion
    }

    public class DivisionWalletModel
    {
        #region [ Constructor ]

        public DivisionWalletModel(Guid walletId, string name)
        {
            WalletId = walletId;
            Name = name;
        }

        #endregion

        #region [ Properties ]

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonElement("WalletId")]
        [BsonRepresentation(BsonType.String)]
        public Guid WalletId { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        #endregion
    }

    public class LimitModel
    {
        #region [ Constructor ]

        public LimitModel(ExpenseType name, double percent)
        {
            LimitId = Guid.NewGuid();
            Name = name;
            Percent = percent;
        }

        #endregion

        #region [ Properties ]

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonElement("LimitId")]
        [BsonRepresentation(BsonType.String)]
        public Guid LimitId { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public ExpenseType Name { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public double Percent { get; set; }

        #endregion

        #region [ Public Methods ]

        public void Update(ExpenseType name, double percent)
        {
            Name = name;
            Percent = percent;
        }

        #endregion
    }
}
