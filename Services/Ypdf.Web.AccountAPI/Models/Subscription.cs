using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ypdf.Web.Domain.Models;

namespace Ypdf.Web.AccoutAPI.Models;

public class Subscription
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public SubscriptionType SubscriptionType { get; set; }
}
