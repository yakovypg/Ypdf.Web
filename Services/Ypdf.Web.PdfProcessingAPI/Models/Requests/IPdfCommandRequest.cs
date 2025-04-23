using System;

namespace Ypdf.Web.PdfProcessingAPI.Models.Requests;

public interface IPdfCommandRequest
{
    Guid UserId { get; set; }
}
