using FinanceControl.Domain.Enuns;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FinanceControl.Domain.Entities
{
    [DataContract]
    [Table("Pix")]
    public class PixModel : EntityBase
    {
        #region [ Constructor ]
        public PixModel(Guid userId, string name, string linkedAccount, PixType type, string color, PixWalletModel wallet)
        {
            PixId = Guid.NewGuid();
            CreatedBy = userId;
            Name = name;
            LinkedAccount = linkedAccount;
            Type = type;
            Color = color;
            Wallet = new PixWalletModel(wallet.WalletId, wallet.Name);
            Active = true;
        }
        public PixModel() { }

        #endregion

        #region [ Properties ]

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonElement("PixId")]
        [BsonRepresentation(BsonType.String)]
        public Guid PixId { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string Color { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public string LinkedAccount { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.String)]
        public PixType Type { get; set; }

        [DataMember]
        [BsonIgnoreIfNull]
        public PixWalletModel Wallet { get; set; }

        #endregion

        #region [ Public Methods ]

        public void Update(string name, string linkedAccount, PixType type, string color)
        {
            Name = name;
            LinkedAccount = linkedAccount;
            Type = type;
            Color = color;
            UpdateDate = DateTime.UtcNow;
        }

        #endregion
    }

    public class PixWalletModel
    {
        #region [ Constructor ]
        public PixWalletModel(Guid walletId, string name)
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
