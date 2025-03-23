using System.ComponentModel.DataAnnotations;

namespace Ypdf.Web.AccoutAPI.Models;

public class Subscription
{
    [Key]
    public int SubscriptionId { get; set; }

    public SubscriptionType SubscriptionType { get; set; }
}
