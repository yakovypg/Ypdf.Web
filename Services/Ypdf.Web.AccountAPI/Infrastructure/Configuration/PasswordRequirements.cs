namespace Ypdf.Web.AccoutAPI.Infrastructure.Configuration;

public sealed class PasswordRequirements
{
    public int MinimumLength { get; set; }
    public int RequiredUniqueChars { get; set; }
    public bool RequireDigit { get; set; }
    public bool RequireLetter { get; set; }
    public bool RequireNonAlphanumeric { get; set; }
    public bool RequireLowercase { get; set; }
    public bool RequireUppercase { get; set; }
}
