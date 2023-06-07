using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using FinanceControl.Application.Extensions.BaseModel;

namespace FinanceControl.Application.Services.User.Model;

[DataContract]
[Table("User")]
public class UserModel : EntityBase
{
    #region [ Constructor ]

    public UserModel()
    {
        
    }

    public UserModel(string name, string email, string password)
    {
        UserId = Guid.NewGuid();
        Name = name;
        Email = email;
        Password = password;
        CreationDate = DateTime.UtcNow;
        Active = true;
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

    [DataMember]
    [BsonIgnoreIfNull]
    public ResetPasswordModel ResetPassword { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public List<FamilyMemberModel> FamilyMembers { get; set; }

    #endregion

    #region [ Public Methods ]

    public void Update(string name, string cellPhone, string occupation, byte[] thumbnail)
    {
        Name = name;
        CellPhone = cellPhone;
        Occupation = occupation;
        Thumbnail = thumbnail;
    }

    public void UpdatePassword(string password)
    {
        Password = password;
    }
    #endregion
}

public class FamilyMemberModel : EntityBase
{
    #region [ Constructor ]

    public FamilyMemberModel()
    {
        FamilyId = Guid.NewGuid();
    }

    #endregion

    #region [ Fields ]

    [DataMember]
    [BsonIgnoreIfNull]
    [BsonElement("FamilyId")]
    [BsonRepresentation(BsonType.String)]
    public Guid FamilyId { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Name { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Email { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public string Kinship { get; set; }
    #endregion
}

public class ResetPasswordModel
{
    [DataMember] 
    [BsonIgnoreIfNull] 
    public bool PasswordReset { get; set; } = true;

    [DataMember]
    [BsonIgnoreIfNull]
    public string Code { get; set; }

    [DataMember]
    [BsonIgnoreIfNull]
    public DateTime ResetDate { get; set; } = DateTime.UtcNow;
}