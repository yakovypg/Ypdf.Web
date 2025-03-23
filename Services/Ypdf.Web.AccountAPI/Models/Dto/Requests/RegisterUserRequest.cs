namespace Ypdf.Web.AccoutAPI.Models.Dto.Requests;

public class RegisterUserRequest
{
    public string? Nickname { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}
