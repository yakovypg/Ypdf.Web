namespace Ypdf.Web.AccoutAPI.Models.Requests;

public class RegisterUserRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Nickname { get; set; }
    public bool UseDefaultNickname { get; set; }
}
