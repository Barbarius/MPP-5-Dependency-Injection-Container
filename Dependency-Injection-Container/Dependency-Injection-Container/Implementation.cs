using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependency_Injection_Container
{
    public class Implementation
    {
        public Type Type { get; private set; }
        public bool IsSingleton { get; private set; }

        public Implementation(Type type, bool isSingleton)
        {
            Type = type;
            IsSingleton = isSingleton;
        }
    }
}
