using System;

namespace FinanceControl.Application.Services.User.DTO_s;

public class FamilyMembersResponse
{
    public Guid FamilyId { get; set; }
    public string Name { get; set; }
    public string Kinship { get; set; }
}