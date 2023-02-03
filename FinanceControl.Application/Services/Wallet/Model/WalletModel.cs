using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using FinanceControl.Extensions.BaseModel;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FinanceControl.Application.Services.Wallet.Model;

[DataContract]
[Table("Wallet")]
public class WalletModel : EntityBase
{
    #region [ Constructor ]
    public WalletModel()
    {
        WalletId = Guid.NewGuid();
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

    [DataMember]
    [BsonIgnoreIfNull]
    public string Color { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public WalletUserModel User { get; set; }
    #endregion

}

public class WalletUserModel
{
    #region [ Constructor ]



    #endregion

    #region [ Properties ]

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonElement("UserId")]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Name { get; set; }
    #endregion
}