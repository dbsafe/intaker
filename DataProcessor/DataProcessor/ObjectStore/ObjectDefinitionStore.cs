using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.ObjectStore
{
    public class ObjectDefinitionStore<TObjectDefinitionType>
    {
        private readonly object _lockObj = new object();
        private readonly Dictionary<string, Type> _store = new Dictionary<string, Type>();

        public string ObjectTypeName { get; }

        public ObjectDefinitionStore(string objectTypeName)
        {
            ObjectTypeName = objectTypeName;
        }

        public TObjectDefinitionType CreateObject(string name)
        {
            var type = FindObject(name);
            return (TObjectDefinitionType)Activator.CreateInstance(type);
        }

        public void Register(string name, Type type)
        {
            var isValidType = type.GetInterfaces().Contains(typeof(TObjectDefinitionType));
            if (!isValidType)
            {
                throw new InvalidOperationException($"{ObjectTypeName} - Type: '{type.Name}' does not implement '{typeof(TObjectDefinitionType).Name}'");
            }

            lock (_lockObj)
            {
                if (_store.ContainsKey(name))
                {
                    throw new InvalidOperationException($"{ObjectTypeName} - A registered {ObjectTypeName} with the name '{name}' already exists");
                }

                _store[name] = type;
            }
        }

        public IEnumerable<KeyValuePair<string, Type>> GetRegisteredObjects()
        {
            lock (_lockObj)
            {
                return _store.ToList();
            }
        }

        private Type FindObject(string name)
        {
            lock (_lockObj)
            {
                if (!_store.ContainsKey(name))
                {
                    throw new InvalidOperationException($"{ObjectTypeName} - '{name}' not found");
                }

                return _store[name];
            }
        }
    }
}
