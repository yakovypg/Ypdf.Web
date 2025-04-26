using System;
using System.Collections.Generic;

namespace Ypdf.Web.FilesAPI.Models.Responses;

public class GetOutputFileResponse
{
    public GetOutputFileResponse(byte[] fileBytes)
    {
        ArgumentNullException.ThrowIfNull(fileBytes, nameof(fileBytes));
        FileBytes = fileBytes;
    }

    public IReadOnlyCollection<byte> FileBytes { get; }
}
