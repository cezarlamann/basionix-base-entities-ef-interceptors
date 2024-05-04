namespace Basionix.BaseEntities.EntityFramework.Interceptors.Tests.Extensions
{
    using FakeItEasy;
    using System;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using TestEntities;

    public class AuditPropertiesConfigurationExtensionsTests
    {
        private AuditableEntityDbContext _context;
        private IDateTimeProvider _dateTimeProvider;
        private IActionContextUserProvider _userProvider;
        private readonly DateTimeOffset _createDate = new(2024, 1, 1, 8, 0, 0, TimeSpan.FromHours(0));
        private readonly DateTimeOffset _updateDate = new(2024, 1, 1, 9, 0, 0, TimeSpan.FromHours(0));
        
        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = A.Fake<IDateTimeProvider>();
            _userProvider = A.Fake<IActionContextUserProvider>();
        }

        [Test]
        public void InitializeContextWithAuditableEntityShouldWorkOk()
        {
            _context = new AuditableEntityDbContext();
            _context.Database.EnsureCreated();
            
            var entity = _context.AuditableEntities.FirstOrDefault(f => f.Id == 1);
            Assert.That(entity, Is.Not.Null);
        }
        
        [Test]
        public void WhenSavingNewAuditableEntityWithTypedInterceptorAndTrackingTheValuesForCreatedByAndCreatedAtShouldBeSet()
        {
            _context = AuditableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateAuditableEntityInterceptor(_dateTimeProvider, _userProvider)
            ]);
            _context.Database.EnsureCreated();
            var user = Guid.NewGuid().ToString();

            A.CallTo(() => _userProvider.GetActionExecutingUser()).Returns(user);
            A.CallTo(() => _dateTimeProvider.UtcNowOffset).Returns(_createDate);
            

            var entity = new AuditableEntity();
            _context.AuditableEntities.Add(entity);
            _context.SaveChanges();
            
            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.GreaterThan(0));
                Assert.That(entity.CreatedAt, Is.EqualTo(_createDate));
                Assert.That(entity.CreatedBy, Is.EqualTo(user));
            });
        }
        
        [Test]
        public void WhenSavingExistingAuditableEntityWithTypedInterceptorAndTrackingTheValuesForLastUpdatedByAndUpdatedAtShouldBeSet()
        {
            _context = AuditableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateAuditableEntityInterceptor(_dateTimeProvider, _userProvider)
            ]);
            _context.Database.EnsureCreated();
            
            var newOwner = Guid.NewGuid().ToString();
            var updatingUser = Guid.NewGuid().ToString();

            A.CallTo(() => _userProvider.GetActionExecutingUser()).Returns(updatingUser);
            A.CallTo(() => _dateTimeProvider.UtcNowOffset).Returns(_updateDate);

            var entity = _context.AuditableEntities.FirstOrDefault(f => f.Id == 1);
            
            Assert.That(entity, Is.Not.Null);
            entity.CreatedBy = newOwner;
            
            _context.SaveChanges();
            
            A.CallTo(() => _userProvider.GetActionExecutingUser()).MustHaveHappened();
            A.CallTo(() => _dateTimeProvider.UtcNowOffset).MustHaveHappened();
            
            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.GreaterThan(0));
                Assert.That(entity.CreatedBy, Is.EqualTo(newOwner));
                Assert.That(entity.LastUpdatedBy, Is.EqualTo(updatingUser));
                Assert.That(entity.UpdatedAt, Is.EqualTo(_updateDate));
            });
        }
        
        [Test]
        public void WhenSavingNewAuditableEntityWithFuncInterceptorAndTrackingTheValuesForCreatedByAndCreatedAtShouldBeSet()
        {
            var user = Guid.NewGuid().ToString();
            
            _context = AuditableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateAuditableEntityFuncInterceptor(() => _createDate, () => user)
            ]);
            
            _context.Database.EnsureCreated();

            var entity = new AuditableEntity();
            _context.AuditableEntities.Add(entity);
            _context.SaveChanges();
            
            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.GreaterThan(0));
                Assert.That(entity.CreatedAt, Is.EqualTo(_createDate));
                Assert.That(entity.CreatedBy, Is.EqualTo(user));
            });
        }
        
        [Test]
        public void WhenSavingExistingAuditableEntityWithFuncInterceptorAndTrackingTheValuesForLastUpdatedByAndUpdatedAtShouldBeSet()
        {
            var updatingUser = Guid.NewGuid().ToString();
            
            _context = AuditableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateAuditableEntityFuncInterceptor(() => _updateDate, () => updatingUser)
            ]);
            _context.Database.EnsureCreated();
            
            var newOwner = Guid.NewGuid().ToString();
            
            var entity = _context.AuditableEntities.FirstOrDefault(f => f.Id == 1);
            
            Assert.That(entity, Is.Not.Null);
            entity.CreatedBy = newOwner;
            
            _context.SaveChanges();
            
            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.GreaterThan(0));
                Assert.That(entity.CreatedBy, Is.EqualTo(newOwner));
                Assert.That(entity.LastUpdatedBy, Is.EqualTo(updatingUser));
                Assert.That(entity.UpdatedAt, Is.EqualTo(_updateDate));
            });
        }
        
                [Test]
        public void WhenSavingNewAuditableEntityWithTypedInterceptorAndNoTrackingTheValuesForCreatedByAndCreatedAtShouldBeSet()
        {
            _context = AuditableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateAuditableEntityInterceptor(_dateTimeProvider, _userProvider)
            ]);
            _context.Database.EnsureCreated();
            var user = Guid.NewGuid().ToString();

            A.CallTo(() => _userProvider.GetActionExecutingUser()).Returns(user);
            A.CallTo(() => _dateTimeProvider.UtcNowOffset).Returns(_createDate);
            

            var entity = new AuditableEntity();
            _context.Entry(entity).State = EntityState.Added;
            _context.SaveChanges();
            
            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.GreaterThan(0));
                Assert.That(entity.CreatedAt, Is.EqualTo(_createDate));
                Assert.That(entity.CreatedBy, Is.EqualTo(user));
            });
        }
        
        [Test]
        public void WhenSavingExistingAuditableEntityWithTypedInterceptorAndNoTrackingTheValuesForLastUpdatedByAndUpdatedAtShouldBeSet()
        {
            _context = AuditableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateAuditableEntityInterceptor(_dateTimeProvider, _userProvider)
            ]);
            _context.Database.EnsureCreated();
            
            var newOwner = Guid.NewGuid().ToString();
            var updatingUser = Guid.NewGuid().ToString();

            A.CallTo(() => _userProvider.GetActionExecutingUser()).Returns(updatingUser);
            A.CallTo(() => _dateTimeProvider.UtcNowOffset).Returns(_updateDate);

            var entity = _context.AuditableEntities.AsNoTracking().FirstOrDefault(f => f.Id == 1);
            Assert.That(entity, Is.Not.Null);
            
            entity.CreatedBy = newOwner;
            
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
            
            A.CallTo(() => _userProvider.GetActionExecutingUser()).MustHaveHappened();
            A.CallTo(() => _dateTimeProvider.UtcNowOffset).MustHaveHappened();
            
            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.GreaterThan(0));
                Assert.That(entity.CreatedBy, Is.EqualTo(newOwner));
                Assert.That(entity.LastUpdatedBy, Is.EqualTo(updatingUser));
                Assert.That(entity.UpdatedAt, Is.EqualTo(_updateDate));
            });
        }
        
        [Test]
        public void WhenSavingNewAuditableEntityWithFuncInterceptorAndNoTrackingTheValuesForCreatedByAndCreatedAtShouldBeSet()
        {
            var user = Guid.NewGuid().ToString();
            
            _context = AuditableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateAuditableEntityFuncInterceptor(() => _createDate, () => user)
            ]);
            
            _context.Database.EnsureCreated();
        
            var entity = new AuditableEntity();
            var entry = _context.Entry(entity); 
            entry.State = EntityState.Added;
            _context.SaveChanges();
            
            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.GreaterThan(0));
                Assert.That(entity.CreatedAt, Is.EqualTo(_createDate));
                Assert.That(entity.CreatedBy, Is.EqualTo(user));
            });
        }
        
        [Test]
        public void WhenSavingExistingAuditableEntityWithFuncInterceptorAndNoTrackingTheValuesForLastUpdatedByAndUpdatedAtShouldBeSet()
        {
            var updatingUser = Guid.NewGuid().ToString();
            
            _context = AuditableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateAuditableEntityFuncInterceptor(() => _updateDate, () => updatingUser)
            ]);
            _context.Database.EnsureCreated();
            
            var newOwner = Guid.NewGuid().ToString();
            
            var entity = _context.AuditableEntities.AsNoTracking().FirstOrDefault(f => f.Id == 1);
            
            Assert.That(entity, Is.Not.Null);
            entity.CreatedBy = newOwner;
            
            var entry = _context.Entry(entity); 
            entry.State = EntityState.Modified;
            _context.SaveChanges();
            
            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.GreaterThan(0));
                Assert.That(entity.CreatedBy, Is.EqualTo(newOwner));
                Assert.That(entity.LastUpdatedBy, Is.EqualTo(updatingUser));
                Assert.That(entity.UpdatedAt, Is.EqualTo(_updateDate));
            });
        }

        [TearDown]
        public void Teardown()
        {
            _context?.Dispose();
        }
    }
}
