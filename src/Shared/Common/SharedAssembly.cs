using System.Reflection;

namespace BlazorCleanArchitecture.Shared.Common
{
    public interface SharedAssembly
    {
        static Assembly Assembly => Assembly.GetExecutingAssembly();
    }
}
