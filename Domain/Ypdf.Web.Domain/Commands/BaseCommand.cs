using System;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Ypdf.Web.Domain.Commands;

public abstract class BaseCommand
{
    protected BaseCommand(IMapper mapper, ILogger<BaseCommand> logger)
    {
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        Mapper = mapper;
        Logger = logger;
    }

    protected IMapper Mapper { get; }
    protected ILogger<BaseCommand> Logger { get; }
}
