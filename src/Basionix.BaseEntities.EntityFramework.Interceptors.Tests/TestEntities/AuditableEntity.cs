namespace Basionix.BaseEntities.EntityFramework.Interceptors.Tests;

using System;

public class AuditableEntity : AbstractEntity<int>, IAmAuditable
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? LastUpdatedBy { get; set; }
}
