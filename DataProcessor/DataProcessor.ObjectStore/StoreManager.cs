using DataProcessor.Domain.Contracts;
using System;
using System.Linq;

namespace DataProcessor.ObjectStore
{
    public class StoreManager
    {
        public static ObjectDefinitionStore<IFieldDecoder> DecoderStore { get; } = new ObjectDefinitionStore<IFieldDecoder>("FieldDecoderStore");

        static StoreManager()
        {
            RegisterDecoders();
        }

        private static void RegisterDecoders()
        {
            RegisterObject<IFieldDecoder, IDecoderRegistry>(DecoderStore);
        }

        private static void RegisterObject<TStoreType, TRegistry>(ObjectDefinitionStore<TStoreType> store)
        {
            Debug($"{store.ObjectTypeName} - Registering objects");

            var decoderRegistryType = typeof(TRegistry);
            var storeType = typeof(TStoreType);

            var registries = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(a => decoderRegistryType.IsAssignableFrom(a) && !a.IsInterface);

            Debug($"{store.ObjectTypeName} - Found {registries.Count()} registries");
            foreach (var registry in registries)
            {
                Debug($"{store.ObjectTypeName} - Creating instance of registry {registry.FullName}");
                var decoderRegistry = (IDecoderRegistry)Activator.CreateInstance(registry);                
                var registeredObjects = decoderRegistry.GetRegisteredObjects();
                Debug($"{store.ObjectTypeName} - {registry.FullName} - Found {registeredObjects.Count()} registered objects");

                foreach (var registeredObject in registeredObjects)
                {
                    Debug($"{store.ObjectTypeName} - {registry.FullName} - Registering object. Name: {registeredObject.Key}, Type: {registeredObject.Value.FullName}");
                    store.Register(registeredObject.Key, registeredObject.Value);
                }
            }
        }

        private static void Debug(string message)
        {
            Domain.Utils.DataProcessorGlobal.Debug($"{nameof(StoreManager)} - {message}");
        }
    }
}
