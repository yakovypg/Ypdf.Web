using System;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Models.Dto.Requests;

public class GetHistoryRequest
{
    public Guid UserId { get; set; }
}
