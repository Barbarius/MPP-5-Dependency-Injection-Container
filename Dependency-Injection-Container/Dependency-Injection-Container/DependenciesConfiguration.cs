using System;
using System.Collections.Generic;
using System.Linq;

namespace Dependency_Injection_Container
{
    public class DependenciesConfiguration
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
            if (dependencyType.IsAssignableFrom(implementationType) 
                || IsAssignableToGenericType(implementationType, dependencyType))
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

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null)
                return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public IEnumerable<Implementation> GetImplementationType(Type type)
        {
            Type collectionType;

            if (type.IsGenericType)
            {
                collectionType = type.GetGenericTypeDefinition();
            }
            else
            {
                collectionType = type;
            }

            if (implementations.TryGetValue(collectionType,
                out List<Implementation> dependencyImplementations))
            {
                IEnumerable<Implementation> result = new List<Implementation>(dependencyImplementations);
                if (type.IsGenericType)
                {
                    result = result.Where((impl) => impl.Type.IsGenericTypeDefinition || type.IsAssignableFrom(impl.Type));
                }

                return result;
            }
            else
            {
                return new List<Implementation>();
            }
        }

        public Implementation GetImplementedType(Type type)
        {
            return implementations.TryGetValue(type, out var implementedTypes)
                ? implementedTypes.FirstOrDefault()
                : null;
        }
    }
}
