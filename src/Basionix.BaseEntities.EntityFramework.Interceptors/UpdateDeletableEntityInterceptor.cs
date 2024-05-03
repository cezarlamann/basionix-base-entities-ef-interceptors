﻿namespace Basionix.BaseEntities.EntityFramework.Interceptors;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Diagnostics;

public class UpdateDeletableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTime;
    private readonly IActionContextUserProvider _userProvider;

    public UpdateDeletableEntityInterceptor(
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

            eventData.Context.UpdateDeletableEntities(utcNow, createdOrModifiedBy);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            var utcNow = _dateTime.UtcNowOffset;
            var createdOrModifiedBy = _userProvider.GetActionExecutingUser();

            eventData.Context.UpdateDeletableEntities(utcNow, createdOrModifiedBy);
        }

        return base.SavingChanges(eventData, result);
    }
}
