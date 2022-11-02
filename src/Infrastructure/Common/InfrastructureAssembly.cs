using System.Reflection;

namespace BlazorCleanArchitecture.Infrastructure.Common
{
    public interface InfrastructureAssembly
    {
        static Assembly Assembly => Assembly.GetExecutingAssembly();
    }
}
