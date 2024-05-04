# Basionix Base Entities - Entity Framework Interceptors
Entity Framework Interceptors for the entity types and interfaces from the Basionix Base Entities project.

In this repository you should find EF Interceptors for the following interfaces:
- `IAmAuditable` (to set Creation/Update dates and the user who performed the actions)
- `IAmSoftDeletable` (to set the `IsDeleted` flag, Deletion date and the user who performed the action)

The user making use of this package and code should register/provide implementations of the interfaces below, as the code in this repository depends on them:
- `IDateTimeProvider` (from the Basionix Base Entities project): There is a base implementation there that always returns a `DateTimeOffset.UtcNow`.
- `IActionContextUserProvider` - The interface used by the interceptors to set the users performing actions when/where needed. The implementations of this interface should make use of `IHttpContextAccessor` in case of actions coming from an API. If the action being accomplished originates from something else (Background jobs, desktop applications, etc.), you should provide the name/code/whatever you'd like to put in the "(Created | Updated | Deleted) by" fields.
- You can also use the constructors that receive delegates for that matter. Just register them accordingly during your DI container setup.

## How to use this package?
- Install the "Basionix Base Entities" from Nuget.org on a project that holds your DB models AND on the project that holds the DbContext for your application, using the command `dotnet add package Basionix.BaseEntities`
- Install it from Nuget.org on a project that holds the DbContext for your application, using the command `dotnet add package Basionix.BaseEntities.EntityFramework.Interceptors`
- Create any class that implements the `IAmAuditable` or `IAmSoftDeletable` interfaces from the project "Basionix Base Entities", like the sample below:
```
    public class AuditableEntity : AbstractEntity<int>, IAmAuditable
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? LastUpdatedBy { get; set; }
    }
    public class SoftDeletableEntity : AbstractEntity<int>, IAmSoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
```
- On your `DbContext`-derived class, inside the `OnModelCreating` override, configure your entities like in this code below:
```
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<AuditableEntity>(e =>
                {
                    e.HasKey(p => p.Id);
                    e.ConfigureAuditProperties();
                });
            modelBuilder.Entity<SoftDeletableEntity>(e => {
                e.HasKey(p => p.Id);
                e.ConfigureSoftDeletableProperties();
            });
        }
```
- Register the interceptors for the entities you'd like to intercept. There are two options for both interceptors:
  - `UpdateAuditableEntityInterceptor(IDateTimeProvider dateTimeProvider, IActionContextUserProvider userProvider)`
  - `UpdateAuditableEntityFuncInterceptor(Func<DateTimeOffset> dateTimeProviderFunc, Func<string> userProviderFunc)`
  - `UpdateDeletableEntityInterceptor(IDateTimeProvider dateTimeProvider, IActionContextUserProvider userProvider)`
  - `UpdateDeletableEntityFuncInterceptor(Func<DateTimeOffset> dateTimeProviderFunc, Func<string> userProviderFunc)`
- Add them to your Entity Framework Options
- Profit!
