#nullable enable
namespace Basionix.BaseEntities.EntityFramework.Interceptors.Tests.TestEntities
{
    using System;

    public class SoftDeletableEntity : AbstractEntity<int>, IAmSoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
