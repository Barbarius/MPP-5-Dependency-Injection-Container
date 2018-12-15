using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependency_Injection_Container
{
    class DependenciesConfiguration
    {
        protected readonly Dictionary<Type, List<Implementation>> implementations;

        public DependenciesConfiguration()
        {
            implementations = new Dictionary<Type, List<Implementation>>;
        }
    }
}
