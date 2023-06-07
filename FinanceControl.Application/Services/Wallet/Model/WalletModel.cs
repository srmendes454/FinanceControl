using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using FinanceControl.Application.Extensions.BaseModel;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FinanceControl.Application.Services.Wallet.Model;

[DataContract]
[Table("Wallet")]
public class WalletModel : EntityBase
{
    #region [ Constructor ]
    public WalletModel(){}

    public WalletModel(string name, string color,  Guid userId, string userName)
    {
        WalletId = Guid.NewGuid();
        Name = name;
        Color = color;
        Active = true;
        CreationDate = DateTime.UtcNow;
        CreatedBy = userId;
        User = new WalletUserModel(userId, userName);
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

    #region [ Public Methods ]

    public void Update(string name, string color)
    {
        Name = name;
        Color = color;
        UpdateDate = DateTime.UtcNow;
    }

    #endregion
}

public class WalletUserModel
{
    #region [ Constructor ]

    public WalletUserModel(Guid userId, string name)
    {
        UserId = userId;
        Name = name;
    }

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