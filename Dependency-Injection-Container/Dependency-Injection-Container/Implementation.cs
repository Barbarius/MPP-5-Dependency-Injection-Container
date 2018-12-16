using System;

namespace Dependency_Injection_Container
{
    public class Implementation
    {
        public Type Type { get; private set; }
        public bool IsSingleton { get; private set; }
        public string Name { get; private set; }
        public object Instance { get; set; }

        public Implementation(Type type, bool isSingleton, string name)
        {
            Type = type;
            IsSingleton = isSingleton;
            Name = name;
            Instance = null;
        }
    }
}
