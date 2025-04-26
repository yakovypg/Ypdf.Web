using System.Collections.Generic;

namespace Ypdf.Web.FilesAPI.Models;

public class SubscriptionAllowedOperations
{
    public SubscriptionAllowedOperations()
    {
        SubscriptionName = string.Empty;
        AllowedOperations = [];
    }

    public string SubscriptionName { get; set; }
    public IEnumerable<string> AllowedOperations { get; set; }
}
