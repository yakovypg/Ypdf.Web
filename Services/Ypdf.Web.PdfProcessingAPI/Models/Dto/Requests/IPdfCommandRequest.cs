using System;

namespace Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;

public interface IPdfCommandRequest
{
    public Guid UserId { get; set; }
}
