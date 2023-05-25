namespace FinanceControl.Application.Services.User.DTO_s;

public class UserInsertRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
public class UserLoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
public class UserUpdateRequest
{
    public string Name { get; set; }
    public string CellPhone { get; set; }
    public string Occupation { get; set; }
    public byte[] Thumbnail { get; set; }
}
public class UserPasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string NewConfirmPassword { get; set; }
}
public class FamilyMemberRequest
{
    public string Name { get; set; }
    public string Kinship { get; set; }
    public string Email { get; set; }
}
public class UserResetPasswordRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
    public string NewConfirmPassword { get; set; }
}
public class UserSendEmailRequest
{
    public string Email { get; set; }
}