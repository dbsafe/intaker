using DataProcessor.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataProcessor.ObjectStore
{
    public class StoreManager
    {
        private static List<string> _loadedAssemblies = new List<string>();
        public static ObjectDefinitionStore<IFieldDecoder> DecoderStore { get; } = new ObjectDefinitionStore<IFieldDecoder>("FieldDecoderStore");
        public static ObjectDefinitionStore<IFieldRule> RuleStore { get; } = new ObjectDefinitionStore<IFieldRule>("FieldRuleStore");

        public static void RegisterObjectsFromAssembly(string path)
        {
            Debug($"Loading assembly from '{path}'");
            var assembly = Assembly.LoadFrom(path);
            Debug($"Registering objects from loaded assembly '{assembly}'");
            RegisterObjectsFromAssembly(assembly);
        }

        static StoreManager()
        {
            RegisterObjects();
        }

        private static void RegisterObjects()
        {
            Debug("Registering objects");

            var registryType = typeof(IObjectRegistry);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetTypes().Any(t =>  registryType.IsAssignableFrom(t) && !t.IsInterface));

            Debug($"Found {assemblies.Count()} loaded assemblies that have an ObjectRegistry");
            foreach (var assembly in assemblies)
            {
                Debug($"Registering objects from assembly '{assembly}'");
                RegisterObjectsFromAssembly(assembly);
            }
        }

        private static void RegisterObjectsFromAssembly(Assembly assembly)
        {
            var isAssemblyAlreadyLoaded = _loadedAssemblies.Any(a => a == assembly.FullName);
            if (isAssemblyAlreadyLoaded)
            {
                Debug($"Objects from assembly '{assembly}' are already registered");
                return;
            }

            _loadedAssemblies.Add(assembly.FullName);

            var decoderRegistryType = typeof(IObjectRegistry);
            var registries = assembly.GetTypes()
                .Where(a => decoderRegistryType.IsAssignableFrom(a) && !a.IsInterface && !a.IsAbstract);

            foreach (var registry in registries)
            {
                Debug($"Registering objects from Registry '{registry.FullName}'");
                RegisterObjectsFromRegistry(registry);
            }
        }

        private static void RegisterObjectsFromRegistry(Type registryType)
        {
            var registry = (IObjectRegistry)Activator.CreateInstance(registryType);

            RegisterObjectsInStore(registryType, registry.GetRegisteredFieldDecoders(), DecoderStore);
            RegisterObjectsInStore(registryType, registry.GetRegisteredFieldRules(), RuleStore);
        }

        private static void RegisterObjectsInStore<TStoreType>(Type registryType, IEnumerable<KeyValuePair<string, Type>> objects, ObjectDefinitionStore<TStoreType> store)
        {
            if (objects == null || objects.Count() == 0)
            {
                Debug($"{registryType.FullName} - No {store.ObjectTypeName} found");
                return;
            }

            foreach (var registeredObject in objects)
            {
                Debug($"{registryType.FullName} - {store.ObjectTypeName} - Registering object. Name: {registeredObject.Key}, Type: {registeredObject.Value.FullName}");
                store.Register(registeredObject.Key, registeredObject.Value);
            }
        }

        private static void Debug(string message)
        {
            Domain.Utils.DataProcessorGlobal.Debug($"{nameof(StoreManager)} - {message}");
        }
    }
}
