using System;

namespace FinanceControl.Application.Services.User.DTO_s;

public class UserResponse
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string CellPhone { get; set; }
    public string Email { get; set; }
    public string Occupation { get; set; }
    public string Thumbnail { get; set; }
}