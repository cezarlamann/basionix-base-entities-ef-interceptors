namespace Basionix.BaseEntities.EntityFramework.Interceptors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Extensions;

    using Interfaces;

    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class UpdateAuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly IDateTimeProvider _dateTime;
        private readonly IActionContextUserProvider _userProvider;
    
        public UpdateAuditableEntityInterceptor(
            IDateTimeProvider dateTimeProvider,
            IActionContextUserProvider userProvider)
        {
            ArgumentNullException.ThrowIfNull(dateTimeProvider);
            ArgumentNullException.ThrowIfNull(userProvider);
            _dateTime = dateTimeProvider;
            _userProvider = userProvider;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                var utcNow = _dateTime.UtcNowOffset;
                var createdOrModifiedBy = _userProvider.GetActionExecutingUser();

                eventData.Context.UpdateAuditableEntities(utcNow, createdOrModifiedBy);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is not null)
            {
                var utcNow = _dateTime.UtcNowOffset;
                var createdOrModifiedBy = _userProvider.GetActionExecutingUser();

                eventData.Context.UpdateAuditableEntities(utcNow, createdOrModifiedBy);
            }

            return base.SavingChanges(eventData, result);
        }

    }
}
