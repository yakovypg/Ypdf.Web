using System;
using System.Collections.Generic;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Ypdf.Web.Domain.Models.Configuration;
using Ypdf.Web.Domain.Models.Informing;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Data.Repositories;

public class PdfOperationResultRepository : IPdfOperationResultRepository, IDisposable
{
    private readonly LiteDatabase _database;
    private readonly ILiteCollection<PdfOperationResult> _operationResults;

    private bool _isDisposed;

    public PdfOperationResultRepository(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        string connectionString = configuration.GetConnectionString("PdfOperationResults")
            ?? throw new ConfigurationException("Connection string to PdfOperationResults not specified");

        _database = new LiteDatabase(connectionString);
        _operationResults = _database.GetCollection<PdfOperationResult>("operation_results");
    }

    ~PdfOperationResultRepository()
    {
        Dispose(false);
    }

    public void Add(PdfOperationResult operationResult)
    {
        ArgumentNullException.ThrowIfNull(operationResult, nameof(operationResult));
        _operationResults.Insert(operationResult);
    }

    public void DeleteById(int id)
    {
        _operationResults.Delete(id);
    }

    public PdfOperationResult GetById(int id)
    {
        return _operationResults.FindById(id);
    }

    public IEnumerable<PdfOperationResult> GetAll()
    {
        return _operationResults.FindAll();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
                _database?.Dispose();

            _isDisposed = true;
        }
    }
}
