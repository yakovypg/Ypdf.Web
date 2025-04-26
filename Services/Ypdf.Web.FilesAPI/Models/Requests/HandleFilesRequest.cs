using System.Collections.Generic;

namespace Ypdf.Web.FilesAPI.Models.Requests;

public class HandleFilesRequest
{
    public IEnumerable<IEnumerable<byte>>? Files { get; set; }
}
