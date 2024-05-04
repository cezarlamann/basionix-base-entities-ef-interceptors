namespace Basionix.BaseEntities.EntityFramework.Interceptors.Tests.Extensions
{
    using FakeItEasy;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using TestEntities;

    public class SoftDeletablePropertiesConfigurationExtensionsTests
    {
        private SoftDeletableEntityDbContext _context;
        private IDateTimeProvider _dateTimeProvider;
        private IActionContextUserProvider _userProvider;
        private readonly DateTimeOffset _deleteDate = new(2024, 1, 1, 8, 0, 0, TimeSpan.FromHours(0));
        
        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = A.Fake<IDateTimeProvider>();
            _userProvider = A.Fake<IActionContextUserProvider>();
        }
        
        [Test]
        public void InitializeContextWithSoftDeletableShouldWorkOk()
        {
            _context = new SoftDeletableEntityDbContext();
            _context.Database.EnsureCreated();
            
            var entity = _context.SoftDeletableEntities.FirstOrDefault(f => f.Id == 1);
            Assert.That(entity, Is.Not.Null);
        }
        
        [Test]
        public void DeleteEntityWithTrackingAndWithoutInterceptorShouldRemoveEntityOk()
        {
            _context = new SoftDeletableEntityDbContext();
            _context.Database.EnsureCreated();
            
            var entity = _context.SoftDeletableEntities.FirstOrDefault(f => f.Id == 1);
            
            Assert.That(entity, Is.Not.Null);

            _context.Remove(entity);
            _context.SaveChanges();

            var deletedEntity = _context.SoftDeletableEntities
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == entity.Id);
            
            Assert.Multiple(() =>
            {
                Assert.That(deletedEntity, Is.Null);
            });
        }
        
        [Test]
        public void DeleteEntityWithTrackingAndTypedInterceptorShouldWorkOk()
        {
            var deletingUser = Guid.NewGuid().ToString();
            A.CallTo(() => _dateTimeProvider.UtcNowOffset).Returns(_deleteDate);
            A.CallTo(() => _userProvider.GetActionExecutingUser()).Returns(deletingUser);
            
            _context = SoftDeletableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateDeletableEntityInterceptor(_dateTimeProvider, _userProvider)
            ]);
            _context.Database.EnsureCreated();
            
            var entity = _context.SoftDeletableEntities.FirstOrDefault(f => f.Id == 1);
            
            Assert.That(entity, Is.Not.Null);

            _context.Remove(entity);
            _context.SaveChanges();

            A.CallTo(() => _dateTimeProvider.UtcNowOffset).MustHaveHappened();
            A.CallTo(() => _userProvider.GetActionExecutingUser()).MustHaveHappened();

            var deletedEntity = _context.SoftDeletableEntities
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == entity.Id);
            
            Assert.Multiple(() =>
            {
                Assert.That(deletedEntity, Is.Not.Null);
                Assert.That(deletedEntity.IsDeleted, Is.True);
                Assert.That(deletedEntity.DeletedAt, Is.EqualTo(_deleteDate));
                Assert.That(deletedEntity.DeletedBy, Is.EqualTo(deletingUser));
            });
        }
        
        [Test]
        public void DeleteEntityWithNoTrackingAndTypedInterceptorShouldWorkOk()
        {
            var deletingUser = Guid.NewGuid().ToString();
            A.CallTo(() => _dateTimeProvider.UtcNowOffset).Returns(_deleteDate);
            A.CallTo(() => _userProvider.GetActionExecutingUser()).Returns(deletingUser);
            
            _context = SoftDeletableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateDeletableEntityInterceptor(_dateTimeProvider, _userProvider)
            ]);
            _context.Database.EnsureCreated();
            
            var entity = _context.SoftDeletableEntities
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == 1);
            
            var entity2 = _context.SoftDeletableEntities
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == 1);
            
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity2, Is.Not.Null);

            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
            _context.SaveChanges();

            A.CallTo(() => _dateTimeProvider.UtcNowOffset).MustHaveHappened();
            A.CallTo(() => _userProvider.GetActionExecutingUser()).MustHaveHappened();

            var deletedEntity = _context.SoftDeletableEntities
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == entity.Id);
            
            Assert.Multiple(() =>
            {
                Assert.That(deletedEntity, Is.Not.Null);
                Assert.That(entity2.IsDeleted, Is.False);
                Assert.That(entity2.DeletedAt, Is.Null);
                Assert.That(entity2.DeletedBy, Is.Null);
                Assert.That(deletedEntity.IsDeleted, Is.True);
                Assert.That(deletedEntity.DeletedAt, Is.EqualTo(_deleteDate));
                Assert.That(deletedEntity.DeletedBy, Is.EqualTo(deletingUser));
            });
        }
        
        [Test]
        public void DeleteEntityWithTrackingAndFuncInterceptorShouldWorkOk()
        {
            var deletingUser = Guid.NewGuid().ToString();
            
            _context = SoftDeletableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateDeletableEntityFuncInterceptor(() => _deleteDate, () => deletingUser)
            ]);
            _context.Database.EnsureCreated();
            
            var entity = _context.SoftDeletableEntities.FirstOrDefault(f => f.Id == 1);
            
            Assert.That(entity, Is.Not.Null);

            _context.Remove(entity);
            _context.SaveChanges();

            var deletedEntity = _context.SoftDeletableEntities
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == entity.Id);
            
            Assert.Multiple(() =>
            {
                Assert.That(deletedEntity, Is.Not.Null);
                Assert.That(deletedEntity.IsDeleted, Is.True);
                Assert.That(deletedEntity.DeletedAt, Is.EqualTo(_deleteDate));
                Assert.That(deletedEntity.DeletedBy, Is.EqualTo(deletingUser));
            });
        }
        
        [Test]
        public void DeleteEntityWithNoTrackingAndFuncInterceptorShouldWorkOk()
        {
            var deletingUser = Guid.NewGuid().ToString();
            
            _context = SoftDeletableEntityDbContext.CreateDbContextWithInterceptors([
                new UpdateDeletableEntityFuncInterceptor(() => _deleteDate, () => deletingUser)
            ]);
            _context.Database.EnsureCreated();
            
            var entity = _context.SoftDeletableEntities
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == 1);
            
            var entity2 = _context.SoftDeletableEntities
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == 1);
            
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity2, Is.Not.Null);

            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
            _context.SaveChanges();

            var deletedEntity = _context.SoftDeletableEntities
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == entity.Id);
            
            Assert.Multiple(() =>
            {
                Assert.That(deletedEntity, Is.Not.Null);
                Assert.That(entity2.IsDeleted, Is.False);
                Assert.That(entity2.DeletedAt, Is.Null);
                Assert.That(entity2.DeletedBy, Is.Null);
                Assert.That(deletedEntity.IsDeleted, Is.True);
                Assert.That(deletedEntity.DeletedAt, Is.EqualTo(_deleteDate));
                Assert.That(deletedEntity.DeletedBy, Is.EqualTo(deletingUser));
            });
        }
        
        [TearDown]
        public void Teardown()
        {
            _context?.Dispose();
        }
    }
}