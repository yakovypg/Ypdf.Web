using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ypdf.Web.AccoutAPI.Models;

public class UserSubscription
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(Subscription))]
    public int SubscriptionId { get; set; }
    [Required]
    public required Subscription Subscription { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    [Required]
    public required User User { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
}
