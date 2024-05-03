namespace Basionix.BaseEntities.EntityFramework.Interceptors;

public interface IActionContextUserProvider
{
    public string GetActionExecutingUser();
}
