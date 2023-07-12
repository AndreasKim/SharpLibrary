﻿namespace Lending.Core.Application.Events;

public record IntegrationEvent
{
    public Guid Id { get; }

    public DateTime CreationDate { get; }

    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
}
