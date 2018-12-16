using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICUnitTests
{
    class TestGenericImpl2<T> : ITestGenericInterface<T>
            where T : ITestInterface
    {
        public T field;

        public TestGenericImpl2(T par)
        {
            field = par;
        }
    }
}
