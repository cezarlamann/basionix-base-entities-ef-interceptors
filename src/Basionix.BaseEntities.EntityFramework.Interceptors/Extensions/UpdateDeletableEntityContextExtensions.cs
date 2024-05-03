namespace Basionix.BaseEntities.EntityFramework.Interceptors.Extensions
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    internal static class UpdateDeletableEntityContextExtensions
    {
        internal static void UpdateDeletableEntities(this DbContext context, DateTimeOffset actionExecutedDateTime, string userWhoPerformedTheAction)
        {
            var entities = context.ChangeTracker
                .Entries<IAmSoftDeletable>()
                .Where(w => w.State == EntityState.Deleted)
                .ToList();

            foreach (EntityEntry<IAmSoftDeletable> entry in entities)
            {
                entry.State = EntityState.Modified;
                entry.SetCurrentPropertyValue(nameof(IAmSoftDeletable.DeletedAt), actionExecutedDateTime);
                entry.SetCurrentPropertyValue(nameof(IAmSoftDeletable.DeletedBy), userWhoPerformedTheAction);
                entry.SetCurrentPropertyValue(nameof(IAmSoftDeletable.IsDeleted), true);
            }        
        }
    }
}
