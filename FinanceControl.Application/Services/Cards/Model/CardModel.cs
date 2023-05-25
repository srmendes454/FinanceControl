using FinanceControl.Application.Services.Cards.Model.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using FinanceControl.Application.Extensions.BaseModel;
using FinanceControl.Application.Extensions.Enum;

namespace FinanceControl.Application.Services.Cards.Model;

[DataContract]
[Table("Card")]
public class CardModel : EntityBase
{
    #region [ Constructor ]
    public CardModel()
    {
        CardId = Guid.NewGuid();
    }
    #endregion

    #region [ Properties ]
    [DataMember]
    [BsonIgnoreIfNull]
    [BsonElement("CardId")]
    [BsonRepresentation(BsonType.String)]
    public Guid CardId { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Name { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Number { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Color { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public int DateExpiration { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public bool Payment { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public double PriceTotal { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public double AvailableLimit { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.String)]
    public Status Status { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.String)]
    public CardType CardType { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public CardWalletModel Wallet { get; set; }
    #endregion
}
public class CardWalletModel
{
    #region [ Constructor ]
    public CardWalletModel()
    {
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