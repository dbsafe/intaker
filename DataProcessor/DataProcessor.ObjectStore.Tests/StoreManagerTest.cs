using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.ObjectStore.Tests.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DataProcessor.ObjectStore.Tests
{
    [TestClass]
    public class StoreManagerTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Utils.DataProcessorGlobal.IsDebugEnabled = true;
            var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var assemblyWithAggregators = Path.Combine(testDirectory, "DataProcessor.ObjectStore.Tests.Lib.dll");
            StoreManager.RegisterObjectsFromAssembly(assemblyWithAggregators);
        }

        [TestMethod]
        public void DecoderStore_Decoders_should_be_registered()
        {
            var registeredDecoders = StoreManager.DecoderStore.GetRegisteredObjects();

            Assert.AreEqual(6, registeredDecoders.Count(), "Expected 6. DataProcessor.dll(4), DataProcessor.ObjectStore.Tests.Lib.dll(1), this project (1)");

            AssertStoredObject("DateDecoder", typeof(Decoders.DateDecoder), registeredDecoders);
            AssertStoredObject("DecimalDecoder", typeof(Decoders.DecimalDecoder), registeredDecoders);
            AssertStoredObject("TextDecoder", typeof(Decoders.TextDecoder), registeredDecoders);
            AssertStoredObject("IntegerDecoder", typeof(Decoders.IntegerDecoder), registeredDecoders);
        }

        [TestMethod]
        public void RuleStore_Rules_should_be_registered()
        {
            var registeredRules = StoreManager.RuleStore.GetRegisteredObjects();
            
            Assert.AreEqual(8, registeredRules.Count(), "Expected 8. DataProcessor.dll(6), DataProcessor.ObjectStore.Tests.Lib.dll(1), this project (1)");

            AssertStoredObject("MinDateFieldRule", typeof(Rules.MinDateFieldRule), registeredRules);
            AssertStoredObject("MaxDateFieldRule", typeof(Rules.MaxDateFieldRule), registeredRules);
            AssertStoredObject("MinNumberFieldRule", typeof(Rules.MinNumberFieldRule), registeredRules);
            AssertStoredObject("MaxNumberFieldRule", typeof(Rules.MaxNumberFieldRule), registeredRules);
            AssertStoredObject("MatchesAggregateRule", typeof(Rules.MatchesAggregateRule), registeredRules);
            AssertStoredObject("RangeNumberFieldRule", typeof(Rules.RangeNumberFieldRule), registeredRules);
        }

        [TestMethod]
        public void AggregatorStore_Aggregators_should_be_registered()
        {
            var registeredAggregators = StoreManager.AggregatorStore.GetRegisteredObjects();

            Assert.AreEqual(4, registeredAggregators.Count(), "Expected 4. DataProcessor.dll(2), DataProcessor.ObjectStore.Tests.Lib.dll(1), this project (1)");

            AssertStoredObject("SumAggregator", typeof(Aggregators.SumAggregator), registeredAggregators);
            AssertStoredObject("RowCountAggregator", typeof(Aggregators.RowCountAggregator), registeredAggregators);
        }

        [TestMethod]
        public void Given_an_objectRegistry_is_defined_in_the_executing_assembly_Objects_should_be_loaded_in_the_store()
        {
            AssertStoredObject("local-decoder", typeof(LocalDecoder), StoreManager.DecoderStore.GetRegisteredObjects());
            AssertStoredObject("local-rule", typeof(LocalRule), StoreManager.RuleStore.GetRegisteredObjects());
            AssertStoredObject("local-aggregator", typeof(LocalAggregator), StoreManager.AggregatorStore.GetRegisteredObjects());
        }

        [TestMethod]
        public void Given_an_objectRegistry_is_defined_in_another_assembly_Objects_should_be_loaded_in_the_store()
        {
            AssertStoredObject("lib-decoder", typeof(LibDecoder), StoreManager.DecoderStore.GetRegisteredObjects());
            AssertStoredObject("lib-rule", typeof(LibRule), StoreManager.RuleStore.GetRegisteredObjects());
            AssertStoredObject("lib-aggregator", typeof(LibAggregator), StoreManager.AggregatorStore.GetRegisteredObjects());
        }

        private void AssertStoredObject(string name, Type type, IEnumerable<KeyValuePair<string, Type>> registeredObjects)
        {
            var objectCount = registeredObjects.Where(a => a.Key == name).Count();
            
            if (objectCount == 0)
            {
                Assert.Fail($"Registered object with name '{name}' not found");
            }

            if (objectCount > 1)
            {
                Assert.Fail($"Registered object with name '{name}' found multiple times");
            }

            var registeredObject = registeredObjects.FirstOrDefault(a => a.Key == name);
            Assert.AreEqual(type, registeredObject.Value, "Unexpected Registered Object type");
        }
    }

    public class LocalObjectRegistry : BaseObjectRegistry
    {
        public static IEnumerable<KeyValuePair<string, Type>> RegisteredFieldDecoders { get; } = new KeyValuePair<string, Type>[]
             {
                new KeyValuePair<string, Type>("local-decoder", typeof(LocalDecoder))
             };

        public static IEnumerable<KeyValuePair<string, Type>> RegisteredFieldRules { get; } = new KeyValuePair<string, Type>[]
             {
                new KeyValuePair<string, Type>("local-rule", typeof(LocalRule))
             };

        public static IEnumerable<KeyValuePair<string, Type>> RegisteredFieldAggregators { get; } = new KeyValuePair<string, Type>[]
             {
                new KeyValuePair<string, Type>("local-aggregator", typeof(LocalAggregator))
             };

        public override IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldDecoders() => RegisteredFieldDecoders;
        public override IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldRules() => RegisteredFieldRules;
        public override IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldAggregators() => RegisteredFieldAggregators;
    }

    public class LocalDecoder : Decoders.FieldDecoder
    {
        public override void Decode(Field field)
        {
            field.ValidationResult = ValidationResultType.Valid;
            field.Value = field.Raw;
        }
    }

    public class LocalRule : Rules.FieldRule { }

    public class LocalAggregator : Aggregators.FieldAggregator
    {
        public override void AggregateField(Field field) { }
    }
}
