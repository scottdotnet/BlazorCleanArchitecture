using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Shared.Common
{
    public interface SharedAssembly
    {
        static Assembly Assembly => Assembly.GetExecutingAssembly();
    }
}
