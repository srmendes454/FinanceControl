using FinanceControl.Domain.Enuns;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FinanceControl.Domain.Entities;

[DataContract]
[Table("Investment")]
public class InvestmentModel : EntityBase
{
    #region [ Constructor ]
    public InvestmentModel(Guid userId, string name, string color, double monthlyProfitability, InvestmentType type, Guid walletId, string walletName)
    {
        InvestmentId = Guid.NewGuid();
        Name = name;
        Color = color;
        MonthlyProfitability = monthlyProfitability;
        Type = type;
        Wallet = new InvestmentWalletModel(walletId, walletName);
        CreatedBy = userId;
        CreationDate = DateTime.Now;
        Active = true;
    }
    public InvestmentModel() { }
    #endregion

    #region [ Properties ]
    [DataMember]
    [BsonIgnoreIfNull]
    [BsonElement("InvestmentId")]
    [BsonRepresentation(BsonType.String)]
    public Guid InvestmentId { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Name { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Color { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public double MonthlyProfitability { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.String)]
    public InvestmentType Type { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public InvestmentWalletModel Wallet { get; set; }
    #endregion

    #region [ Public Methods ]

    public void Update(string name, string color, double monthlyProfitability, InvestmentType type)
    {
        Name = name;
        Color = color;
        MonthlyProfitability = monthlyProfitability;
        Type = type;
        UpdateDate = DateTime.UtcNow;
    }

    #endregion
}
public class InvestmentWalletModel
{
    #region [ Constructor ]

    public InvestmentWalletModel(Guid walletId, string name)
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