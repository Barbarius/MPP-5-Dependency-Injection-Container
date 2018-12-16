using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependency_Injection_Container;
using System.Collections.Generic;
using System.Linq;

namespace DICUnitTests
{
    [TestClass]
    public class DICUnitTest
    {
        DependenciesConfiguration dependenciesConfiguration;
        DependencyProvider dependencyProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            dependenciesConfiguration = new DependenciesConfiguration();
        }

        [TestMethod]
        public void GenericRegisterTest()
        {
            dependenciesConfiguration.Register<ITestGenericInterface<ITestInterface>, TestGenericImpl1<ITestInterface>>();
            dependenciesConfiguration.Register<ITestGenericInterface<ITestInterface>, TestGenericImpl2<ITestInterface>>();
            var registeredImplementations = dependenciesConfiguration.GetImplementationType(typeof(ITestGenericInterface<ITestInterface>)).ToList();
            Assert.AreEqual(2, registeredImplementations.Count);

            List<Type> expectedRegisteredTypes = new List<Type>
            {
                typeof(TestGenericImpl1<ITestInterface>),
                typeof(TestGenericImpl2<ITestInterface>)
            };
            CollectionAssert.AreEquivalent(expectedRegisteredTypes,
                registeredImplementations.Select((implementation) => implementation.Type).ToList());
        }

        [TestMethod]
        public void OpenGenericRegisterTest()
        {
            dependenciesConfiguration.Register(typeof(ITestGenericInterface<>), typeof(TestGenericImpl1<>));
            dependenciesConfiguration.Register(typeof(ITestGenericInterface<>), typeof(TestGenericImpl2<>));
            var registeredImplementations = dependenciesConfiguration.GetImplementationType(typeof(ITestGenericInterface<>)).ToList();
            Assert.AreEqual(2, registeredImplementations.Count);

            List<Type> expectedRegisteredTypes = new List<Type>
            {
                typeof(TestGenericImpl1<>),
                typeof(TestGenericImpl2<>)
            };
            CollectionAssert.AreEquivalent(expectedRegisteredTypes,
                registeredImplementations.Select((implementation) => implementation.Type).ToList());
        }

        [TestMethod]
        public void NonGenericRegisterTest()
        {
            dependenciesConfiguration.Register<ITestInterface, TestImpl1>();
            dependenciesConfiguration.Register<ITestInterface, TestImpl2>();
            var registeredImplementations = dependenciesConfiguration.GetImplementationType(typeof(ITestInterface)).ToList();
            Assert.AreEqual(2, registeredImplementations.Count);

            List<Type> expectedRegisteredTypes = new List<Type>
            {
                typeof(TestImpl1),
                typeof(TestImpl2)
            };
            CollectionAssert.AreEquivalent(expectedRegisteredTypes,
                registeredImplementations.Select((implementation) => implementation.Type).ToList());
        }
    }
}
