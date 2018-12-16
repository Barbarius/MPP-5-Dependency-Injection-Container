using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependency_Injection_Container
{
    public class DependencyKey : Attribute
    {
        public string Name
        { get; protected set; }

        public DependencyKey(string name)
        {
            Name = name;
        }
    }
}
