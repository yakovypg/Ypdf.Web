using System;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Models.Requests;

public class GetHistoryRequest
{
    public Guid UserId { get; set; }
}
