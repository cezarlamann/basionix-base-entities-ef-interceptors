namespace Basionix.BaseEntities.EntityFramework.Interceptors.Extensions
{
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    internal static class EntityEntryExtensions
    {
        internal static void SetCurrentPropertyValue(this EntityEntry entry,
            string propertyName,
            object? value) =>
            entry.Property(propertyName).CurrentValue = value;
    }
}
