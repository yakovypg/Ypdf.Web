using System.Net;

namespace Ypdf.Web.Domain.Models.Api;

public record ApiError(HttpStatusCode StatusCode, string Message);
