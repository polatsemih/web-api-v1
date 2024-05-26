namespace SUPBank.Domain.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class DapperIgnoreAttribute : Attribute
    {

    }
}
