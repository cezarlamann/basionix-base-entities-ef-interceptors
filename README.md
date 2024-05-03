# Basionix Base Entities - Entity Framework Interceptors
Entity Framework Interceptors for the entity types and interfaces from the Basionix Base Entities project.

In this repository you should find EF Interceptors for the following interfaces:
- `IAmAuditable` (to set Creation/Update dates and the user who performed the actions)
- `IAmSoftDeletable` (to set the `IsDeleted` flag, Deletion date and the user who performed the action)

The user making use of this package and code should register/provide implementations of the interfaces below, as the code in this repository depends on them:
- `IDateTimeProvider` (from the Basionix Base Entities project): There is a base implementation there that always returns a `DateTimeOffset.UtcNow`.
- `IActionContextUserProvider` - The interface used by the interceptors to set the users performing actions when/where needed. The implementations of this interface should make use of `IHttpContextAccessor` in case of actions coming from an API. If the action being accomplished originates from something else (Background jobs, desktop applications, etc.), you should provide the name/code/whatever you'd like to put in the "(Created | Updated | Deleted) by" fields.
- You can also use the constructors that receive delegates for that matter. Just register them accordingly during your DI container setup.
