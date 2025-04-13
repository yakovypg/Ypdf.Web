using System.Collections.Generic;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Data.Repositories;

public interface IPdfOperationResultRepository
{
    void Add(PdfOperationResult operationResult);
    void DeleteById(int id);

    PdfOperationResult GetById(int id);
    IEnumerable<PdfOperationResult> GetAll();
}
