using FinanceControl.Domain.Enuns;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FinanceControl.Domain.Entities
{
    [DataContract]
    [Table("AccountBank")]
    public class AccountBankModel : EntityBase
    {
        #region [ Constructor ]
        public AccountBankModel(Guid userId, string name, AccountBankType type, string color, AccountBankWalletModel wallet)
        {
            AccountBankId = Guid.NewGuid();
            CreatedBy = userId;
            Name = name;
            Type = type;
            Color = color;
            Wallet = new AccountBankWalletModel(wallet.WalletId, wallet.Name);
            Active = true;
        }
        public AccountBankModel() { }

        #endregion

        #region [ Properties ]

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonElement("AccountBankId")]
        [BsonRepresentation(BsonType.String)]
        public Guid AccountBankId { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Color { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public AccountBankType Type { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public AccountBankWalletModel Wallet { get; set; }

        #endregion

        #region [ Public Methods ]

        public void Update(string name, AccountBankType type, string color)
        {
            Name = name;
            Type = type;
            Color = color;
            UpdateDate = DateTime.UtcNow;
        }

        #endregion
    }

    public class AccountBankWalletModel
    {
        #region [ Constructor ]
        public AccountBankWalletModel(Guid walletId, string name)
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
}
