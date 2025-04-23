using Microsoft.AspNetCore.Identity;

namespace Ypdf.Web.AccoutAPI.Models;

public class User : IdentityUser<int>
{
    public int UserSubscriptionId { get; set; }
    public UserSubscription? UserSubscription { get; set; }
}
