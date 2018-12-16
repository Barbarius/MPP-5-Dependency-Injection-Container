using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection;
using System.Collections.Concurrent;
using System.Threading;

namespace Dependency_Injection_Container
{
    public class DependencyProvider
    {
        protected readonly DependenciesConfiguration dependenciesConfiguration;
        private Stack<Type> stack;
        //private readonly Stack<Type> stack;
        private static readonly object syncObj = new object();

        public DependencyProvider(DependenciesConfiguration dc)
        {
            dependenciesConfiguration = dc;
            stack = new Stack<Type>();
        }

        public IEnumerable<TDependency> Resolve<TDependency>(string name = null)
            where TDependency : class
        {
            lock (syncObj)
            {
                var dependencyType = typeof(TDependency);

                stack = new Stack<Type>();

                return Resolve(dependencyType, name).OfType<TDependency>();
            }
        }

        private IEnumerable<object> Resolve(Type dependency, string name)
        {
            if (dependency.IsGenericType || dependency.IsGenericTypeDefinition)
            {
                return ResolveGeneric(dependency, name);
            }
            else
            {
                return ResolveNonGeneric(dependency, name);
            }
        }

        private IEnumerable<object> ResolveGeneric(Type dependency, string name)
        {
            List<object> result = new List<object>();
            IEnumerable<Implementation> implementationContainers 
                = dependenciesConfiguration.GetImplementationType(dependency)
                .Where((impl) => !stack.Contains(impl.Type));

            if (name != null)
            {
                implementationContainers = implementationContainers
                    .Where((implementation) => implementation.Name == name);
            }

            object instance;

            foreach (Implementation implementationContainer in implementationContainers)
            {
                instance = CreateInstanceByConstructor(implementationContainer.Type.GetGenericTypeDefinition()
                    .MakeGenericType(dependency.GenericTypeArguments));

                if (instance != null)
                {
                    result.Add(instance);
                }
            }

            return result;
        }

        private IEnumerable<object> ResolveNonGeneric(Type dependency, string name)
        {
            if (dependency.IsValueType)
            {
                return new List<object>
                {
                    Activator.CreateInstance(dependency)
                };
            }

            List<object> result = new List<object>();
            IEnumerable<Implementation> implementationContainers
               = dependenciesConfiguration.GetImplementationType(dependency)
                .Where((impl) => !stack.Contains(impl.Type));

            if (name != null)
            {
                implementationContainers = implementationContainers
                    .Where((implementation) => implementation.Name == name);
            }
            
            object dependencyInstance;

            foreach (Implementation container in implementationContainers)
            {
                if (container.IsSingleton)
                {
                    if (container.Instance == null)
                    {
                        lock (container)
                        {
                            if (container.Instance == null)
                            {
                                container.Instance = CreateInstanceByConstructor(container.Type);
                            }
                        }
                    }
                    dependencyInstance = container.Instance;
                }
                else
                {
                    dependencyInstance = CreateInstanceByConstructor(container.Type);
                }

                if (dependencyInstance != null)
                {
                    result.Add(dependencyInstance);
                }
            }
            return result;
        }

        protected object CreateInstanceByConstructor(Type type)
        {
            if (stack.Contains(type))
            {
                throw new Exception("Circular dependency");
            }

            ConstructorInfo[] constructors = type.GetConstructors().OrderBy((constructor) => constructor.GetParameters().Length).ToArray();
            object instance = null;
            List<object> parameters = new List<object>();

            stack.Push(type);

            for (int constructor = 0; (constructor < constructors.Length) && (instance == null); ++constructor)
            {
                try
                {
                    foreach (ParameterInfo constructorParameter in constructors[constructor].GetParameters())
                    {
                        parameters.Add(Resolve(constructorParameter.ParameterType,
                            constructorParameter.GetCustomAttribute<DependencyKey>()?.Name).FirstOrDefault());
                    }
                    instance = constructors[constructor].Invoke(parameters.ToArray());
                }
                catch
                {
                    parameters.Clear();
                }
            }

            stack.Pop();

            return instance;
        }
    }
}
