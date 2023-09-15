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
    public CardModel(Guid userId, string name, string color, int dateExpiration, int closingDay, Status status, CardType type, CardWalletModel wallet)
    {
        CardId = Guid.NewGuid();
        Name = name;
        Color = color;
        DateExpiration = dateExpiration;
        ClosingDay = closingDay;
        Payment = false;
        Status = status;
        Type = type;
        Wallet = new CardWalletModel(wallet.WalletId, wallet.Name);
        CreatedBy = userId;
        CreationDate = DateTime.Now;
        Active = true;
    }
    public CardModel(){}
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
    public string Color { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public int DateExpiration { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public int ClosingDay { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public bool Payment { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.String)]
    public Status Status { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.String)]
    public CardType Type { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public CardWalletModel Wallet { get; set; }
    #endregion

    #region [ Public Methods ]

    public void Update(string name, string color, int dateExpiration, CardType type)
    {
        Name = name;
        Color = color;
        DateExpiration = dateExpiration;
        Type = type;
        UpdateDate = DateTime.UtcNow;
    }

    public void UpdatePayment ()
    {
        Payment = true;
        Status = Status.PAID_OUT;
        UpdateDate = DateTime.UtcNow;
    }

    #endregion
}
public class CardWalletModel
{
    #region [ Constructor ]
    public CardWalletModel(Guid walletId, string name)
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