using FinanceControl.Application.Extensions.BaseModel;
using FinanceControl.Application.Extensions.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FinanceControl.Application.Services.CardBill.Model;

[DataContract]
[Table("CardBill")]
public class CardBillModel : EntityBase
{
    #region [ Constructor ]
    public CardBillModel(Guid userId, DateTime expirationDate, CardBillCardCredit card)
    {
        CardBillId = Guid.NewGuid();
        ExpirationDate = expirationDate;
        CreatedBy = userId;
        CreationDate = DateTime.Now;
        Active = true;
        Card = new CardBillCardCredit(card.CardId, card.Name);
    }
    public CardBillModel() { }
    #endregion

    #region [ Properties ]
    [DataMember]
    [BsonIgnoreIfNull]
    [BsonElement("CardBillId")]
    [BsonRepresentation(BsonType.String)]
    public Guid CardBillId { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public DateTime ExpirationDate { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public double TotalValue { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.String)]
    public Status Status { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public CardBillCardCredit Card { get; set; }

    #endregion
}

public class CardBillCardCredit
{
    #region [ Constructor ]
    public CardBillCardCredit(Guid cardId, string name)
    {
        CardId = cardId;
        Name = name;
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

    #endregion
}