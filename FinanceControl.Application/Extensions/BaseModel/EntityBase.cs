using System;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinanceControl.Extensions.BaseModel;

[DataContract]
[BsonIgnoreExtraElements]
public class EntityBase
{
    [DataMember]
    [BsonIgnoreIfNull]
    [BsonElement("Active")]
    [BsonRepresentation(BsonType.Boolean)]
    public bool Active { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonElement("CreatedBy")]
    [BsonRepresentation(BsonType.String)]
    public Guid? CreatedBy { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonElement("CreationDate")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? CreationDate { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonElement("UpdateDate")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? UpdateDate { get; set; }
}