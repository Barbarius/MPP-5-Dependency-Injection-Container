using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dependency_Injection_Container;

namespace DICUnitTests
{
    class NamedConstructImpl : ITestInterface
    {
        public readonly ITestInterface intfImpl1;
        public readonly ITestInterface intfImpl2;

        public NamedConstructImpl([DependencyKey("1")] ITestInterface intfImpl1,
            [DependencyKey("2")] ITestInterface intfImpl2)
        {
            this.intfImpl1 = intfImpl1;
            this.intfImpl2 = intfImpl2;
        }
    }
}
