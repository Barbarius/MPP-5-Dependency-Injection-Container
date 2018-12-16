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

        [TestMethod]
        public void GenericResolveTest()
        {
            dependenciesConfiguration.Register<ITestGenericInterface<ITestInterface>, TestGenericImpl1<ITestInterface>>();
            dependenciesConfiguration.Register<ITestGenericInterface<ITestInterface>, TestGenericImpl2<ITestInterface>>();
            dependenciesConfiguration.Register<ITestGenericInterface<ITestInterface>, TestGenericImpl3<ITestInterface>>();
            dependencyProvider = new DependencyProvider(dependenciesConfiguration);
            var resolvedInstances = dependencyProvider.Resolve<ITestGenericInterface<ITestInterface>>();
            Assert.AreEqual(3, resolvedInstances.Count());

            List<Type> expectedRegisteredTypes = new List<Type>
            {
                typeof(TestGenericImpl1<ITestInterface>),
                typeof(TestGenericImpl2<ITestInterface>),
                typeof(TestGenericImpl3<ITestInterface>)
            };
            CollectionAssert.AreEquivalent(expectedRegisteredTypes,
                resolvedInstances.Select((resolvedInstance) => resolvedInstance.GetType()).ToList());
        }

        [TestMethod]
        public void OpenGenericResolveTest()
        {
            dependenciesConfiguration.Register<ITestInterface, TestImpl2>();

            dependenciesConfiguration.Register(typeof(ITestGenericInterface<>), typeof(TestGenericImpl1<>));
            dependenciesConfiguration.Register(typeof(ITestGenericInterface<>), typeof(TestGenericImpl2<>));
            dependenciesConfiguration.Register(typeof(ITestGenericInterface<>), typeof(TestGenericImpl3<>));
            dependencyProvider = new DependencyProvider(dependenciesConfiguration);
            var resolvedInstances = dependencyProvider.Resolve<ITestGenericInterface<ITestInterface>>();
            Assert.AreEqual(3, resolvedInstances.Count());

            List<Type> expectedRegisteredTypes = new List<Type>
            {
                typeof(TestGenericImpl1<ITestInterface>),
                typeof(TestGenericImpl2<ITestInterface>),
                typeof(TestGenericImpl3<ITestInterface>)
            };
            CollectionAssert.AreEquivalent(expectedRegisteredTypes,
                resolvedInstances.Select((resolvedInstance) => resolvedInstance.GetType()).ToList());

            Assert.AreEqual(typeof(TestImpl2),
                resolvedInstances.OfType<TestGenericImpl2<ITestInterface>>().First().field.GetType());
        }

        [TestMethod]
        public void NonGenericResolveTest()
        {
            dependenciesConfiguration.Register<ITestInterface, TestImpl1>();
            dependenciesConfiguration.Register<ITestInterface, TestImpl2>();
            dependencyProvider = new DependencyProvider(dependenciesConfiguration);
            var resolvedInstances = dependencyProvider.Resolve<ITestInterface>();
            Assert.AreEqual(2, resolvedInstances.Count());

            List<Type> expectedRegisteredTypes = new List<Type>
            {
                typeof(TestImpl1),
                typeof(TestImpl2)
            };
            CollectionAssert.AreEquivalent(expectedRegisteredTypes,
                resolvedInstances.Select((resolvedInstance) => resolvedInstance.GetType()).ToList());
        }

        [TestMethod]
        public void SingletonResolveTest()
        {
            dependenciesConfiguration.Register<ITestInterface, TestImpl1>(true);
            dependencyProvider = new DependencyProvider(dependenciesConfiguration);

            Assert.ReferenceEquals(dependencyProvider.Resolve<ITestInterface>().First(),
                dependencyProvider.Resolve<ITestInterface>().First());
        }

        [TestMethod]
        public void NamedConstructorResolveTest()
        {
            dependenciesConfiguration.Register<ITestInterface, TestImpl1>(name: "1");
            dependenciesConfiguration.Register<ITestInterface, TestImpl2>(name: "2");
            dependenciesConfiguration.Register<ITestInterface, NamedConstructImpl>();
            dependencyProvider = new DependencyProvider(dependenciesConfiguration);
            var instances = dependencyProvider.Resolve<ITestInterface>().OfType<NamedConstructImpl>();

            Assert.AreEqual(1, instances.Count());
            Assert.AreEqual(typeof(TestImpl1), instances.First().intfImpl1.GetType());
            Assert.AreEqual(typeof(TestImpl2), instances.First().intfImpl2.GetType());
        }
    }
}
