namespace Basionix.BaseEntities.EntityFramework.Interceptors.Tests;

public class AuditPropertiesConfigurationExtensionsTests
{
    private AuditableEntityDbContext _context;

    [SetUp]
    public void Setup()
    {
        _context = new AuditableEntityDbContext();
    }

    [Test]
    public void InitializeContextWithAuditableEntityShouldWorkOk()
    {
        var entity = _context.AuditableEntities.FirstOrDefault(f => f.Id == 1);

        Assert.That(entity, Is.Not.Null);
    }

    [TearDown]
    public void Teardown()
    {
        _context.Dispose();
    }
}
