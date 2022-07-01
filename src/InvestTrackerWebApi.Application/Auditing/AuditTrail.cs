namespace InvestTrackerWebApi.Application.Auditing;
using System;
using System.Collections.Generic;
using InvestTrackerWebApi.Application.Serialization;
using InvestTrackerWebApi.Domain.Audit;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class AuditTrail
{
    private readonly ISerializerService serializer;

    public AuditTrail(EntityEntry entry, ISerializerService serializer)
    {
        this.Entry = entry;
        this.serializer = serializer;
    }

    public EntityEntry Entry { get; }
    public Guid UserId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public Dictionary<string, object?> KeyValues { get; } = new();
    public Dictionary<string, object?> OldValues { get; } = new();
    public Dictionary<string, object?> NewValues { get; } = new();
    public List<PropertyEntry> TemporaryProperties { get; } = new();
    public TrailType TrailType { get; set; }
    public List<string> ChangedColumns { get; } = new();
    public bool HasTemporaryProperties => this.TemporaryProperties.Count > 0;

    public Trail ToAuditTrail()
    {
        var trail = new Trail(
            Guid.NewGuid(),
            this.UserId,
            this.TrailType.ToString(),
            this.TableName,
            DateTime.UtcNow,
            this.OldValues.Count == 0 ? string.Empty : this.serializer.Serialize(this.OldValues),
            this.NewValues.Count == 0 ? string.Empty : this.serializer.Serialize(this.NewValues),
            this.ChangedColumns.Count == 0 ? string.Empty : this.serializer.Serialize(this.ChangedColumns),
            this.serializer.Serialize(this.KeyValues));
        return trail;
    }
}

public enum TrailType : byte
{
    None = 0,
    Create = 1,
    Update = 2,
    Delete = 3
}
