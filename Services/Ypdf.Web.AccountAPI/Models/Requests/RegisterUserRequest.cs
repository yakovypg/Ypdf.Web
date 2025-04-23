namespace Ypdf.Web.AccoutAPI.Models.Requests;

public class RegisterUserRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? UserName { get; set; }
    public bool UseDefaultUserName { get; set; }
}
