using System;

namespace Ypdf.Web.PdfProcessingAPI.Models.Dto.Requests;

public interface IPdfCommandRequest
{
    Guid UserId { get; set; }
}
