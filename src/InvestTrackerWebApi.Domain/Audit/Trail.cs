namespace InvestTrackerWebApi.Domain.Audit;
using System;
using InvestTrackerWebApi.Domain.Common;

public class Trail : BaseEntity<Guid>
{
    public Trail(Guid id, Guid userId, string type, string tableName, DateTime dateTime, string oldValues, string newValues, string affectedColumns, string primaryKey)
        : base(id)
    {
        this.UserId = userId;
        this.Type = type;
        this.TableName = tableName;
        this.DateTime = dateTime;
        this.OldValues = oldValues;
        this.NewValues = newValues;
        this.AffectedColumns = affectedColumns;
        this.PrimaryKey = primaryKey;
    }

    // used by ef core. should not be used for development.
    internal Trail()
    {
    }

    public Guid UserId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string TableName { get; private set; } = string.Empty;
    public DateTime DateTime { get; private set; }
    public string OldValues { get; private set; } = string.Empty;
    public string NewValues { get; private set; } = string.Empty;
    public string AffectedColumns { get; private set; } = string.Empty;
    public string PrimaryKey { get; private set; } = string.Empty;
}
