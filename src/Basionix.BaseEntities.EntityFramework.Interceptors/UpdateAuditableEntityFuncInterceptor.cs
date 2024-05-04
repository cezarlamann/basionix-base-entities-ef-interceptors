namespace Basionix.BaseEntities.EntityFramework.Interceptors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Extensions;

    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class UpdateAuditableEntityFuncInterceptor : SaveChangesInterceptor
    {
        private readonly Func<DateTimeOffset> _dateTimeFunc;
        private readonly Func<string> _userProviderFunc;
    
        public UpdateAuditableEntityFuncInterceptor(
            Func<DateTimeOffset> dateTimeProviderFunc,
            Func<string> userProviderFunc)
        {
            ArgumentNullException.ThrowIfNull(dateTimeProviderFunc);
            ArgumentNullException.ThrowIfNull(userProviderFunc);
            _dateTimeFunc = dateTimeProviderFunc;
            _userProviderFunc = userProviderFunc;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                var utcNow = _dateTimeFunc();
                var createdOrModifiedBy = _userProviderFunc();

                eventData.Context.UpdateAuditableEntities(utcNow, createdOrModifiedBy);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is not null)
            {
                var utcNow = _dateTimeFunc();
                var createdOrModifiedBy = _userProviderFunc();

                eventData.Context.UpdateAuditableEntities(utcNow, createdOrModifiedBy);
            }

            return base.SavingChanges(eventData, result);
        }
    }
}
