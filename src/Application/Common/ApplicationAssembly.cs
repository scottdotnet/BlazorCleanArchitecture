using System.Reflection;

namespace BlazorCleanArchitecture.Application.Common
{
    public interface ApplicationAssembly
    {
        static Assembly Assembly => Assembly.GetExecutingAssembly();
    }
}
