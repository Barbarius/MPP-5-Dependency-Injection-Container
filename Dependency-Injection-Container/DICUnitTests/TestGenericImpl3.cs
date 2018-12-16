using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICUnitTests
{
    class TestGenericImpl3<T> : ITestGenericInterface<T>
        where T : ITestInterface
    {
    }
}
