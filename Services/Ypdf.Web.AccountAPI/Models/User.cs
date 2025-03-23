using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ypdf.Web.AccoutAPI.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [ForeignKey("SubscriptionId")]
    public Subscription? Subscription { get; set; }

    public string? Nickname { get; set; }
}
