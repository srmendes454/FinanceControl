using FinanceControl.Extensions.BaseModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FinanceControl.Application.Services.User.Model;

[DataContract]
[Table("User")]
public class UserModel : EntityBase
{
    #region [ Constructor ]
    public UserModel()
    {
        UserId = Guid.NewGuid();
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

    [DataMember]
    [BsonIgnoreIfNull]
    public string Email { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string CellPhone { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Occupation { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Password { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public byte[] Thumbnail { get; set; }

    #endregion
}