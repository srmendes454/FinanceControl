using FinanceControl.Application.Extensions.BaseModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FinanceControl.Application.Services.BankSlip.Model
{
    [DataContract]
    [Table("BankSlip")]
    public class BankSlipModel : EntityBase
    {
        #region [ Constructor ]
        public BankSlipModel(Guid userId, string name, int expirationDay, BankSlipWalletModel wallet)
        {
            BankSlipId = Guid.NewGuid();
            CreatedBy = userId;
            Name = name;
            ExpirationDay = expirationDay;
            Wallet = new BankSlipWalletModel(wallet.WalletId, wallet.Name);
            Active = true;
        }
        public BankSlipModel() { }

        #endregion

        #region [ Properties ]

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonElement("BankSlipId")]
        [BsonRepresentation(BsonType.String)]
        public Guid BankSlipId { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public int ExpirationDay { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public BankSlipWalletModel Wallet { get; set; }

        #endregion

        #region [ Public Methods ]

        public void Update(string name, int expirationDay)
        {
            Name = name;
            ExpirationDay = expirationDay;
            UpdateDate = DateTime.UtcNow;
        }

        #endregion
    }

    public class BankSlipWalletModel
    {
        #region [ Constructor ]
        public BankSlipWalletModel(Guid walletId, string name)
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
