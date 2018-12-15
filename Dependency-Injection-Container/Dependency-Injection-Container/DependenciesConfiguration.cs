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
            implementations = new Dictionary<Type, List<Implementation>>();
        }

        public void Register<TDependency, TImplementation>(bool isSingleton = false, string name = null)
            where TDependency : class
            where TImplementation : TDependency
        {
            Register(typeof(TDependency), typeof(TImplementation), isSingleton, name);
        }

        public void Register(Type dependencyType, Type implementationType, bool isSingleton = false, string name = null)
        {
            if (!dependencyType.IsAssignableFrom(implementationType))
            {
                Implementation tempImplementation = new Implementation(implementationType, isSingleton, name);

                if (dependencyType.IsGenericType)
                {
                    dependencyType = dependencyType.GetGenericTypeDefinition();
                }

                List<Implementation> dependencyImplementations;

                lock (implementations)
                {
                    if(!implementations.TryGetValue(dependencyType, out dependencyImplementations))
                    {
                        dependencyImplementations = new List<Implementation>();
                        implementations[dependencyType] = dependencyImplementations;
                    }
                }

                lock (dependencyImplementations)
                {
                    if (name != null)
                    {
                        dependencyImplementations.RemoveAll((existingImplementation) => existingImplementation.Name == name);
                    }
                    dependencyImplementations.Add(tempImplementation);
                }
            }
            else
            {
                throw new ApplicationException("Types are incompatible");
            }
        }
    }
}
