namespace Basionix.BaseEntities.EntityFramework.Interceptors.Extensions
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    internal static class UpdateAuditableEntityContextExtensions
    {
        internal static void UpdateAuditableEntities(this DbContext context, DateTimeOffset actionExecutedDateTime, string userWhoPerformedTheAction)
        {
            var entities = context.ChangeTracker.Entries<IAmAuditable>().ToList();

            foreach (EntityEntry<IAmAuditable> entry in entities)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.SetCurrentPropertyValue(nameof(IAmAuditable.CreatedAt), actionExecutedDateTime);
                    entry.SetCurrentPropertyValue(nameof(IAmAuditable.CreatedBy), userWhoPerformedTheAction);
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.SetCurrentPropertyValue(nameof(IAmAuditable.UpdatedAt), actionExecutedDateTime);
                    entry.SetCurrentPropertyValue(nameof(IAmAuditable.LastUpdatedBy), userWhoPerformedTheAction);
                }
            }
        }
    }
}
