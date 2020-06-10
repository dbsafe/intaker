using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
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
        private readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [TestInitialize]
        public void Initialize()
        {
            Domain.Utils.DataProcessorGlobal.IsDebugEnabled = true;
        }

        [TestMethod]
        public void DecoderStore_Decoders_should_be_registered()
        {
            var actual = StoreManager.DecoderStore.GetRegisteredObjects().ToList();

            var numberOfDecodersRegisteredInThisTest = TestDecoderRegistry.RegisteredFieldDecoders.Count();
            var objectRegistry = new Decoders.ObjectRegistry();
            var numberOfDecodersRegisteredInTheDecoderRegistry = objectRegistry.GetRegisteredFieldDecoders().Count();

            Assert.AreEqual(numberOfDecodersRegisteredInThisTest + numberOfDecodersRegisteredInTheDecoderRegistry, actual.Count);

            Assert.AreEqual("Test-FieldA", actual[0].Key);
            Assert.AreEqual(typeof(TestFieldDecoder), actual[0].Value);

            Assert.AreEqual("Test-FieldB", actual[1].Key);
            Assert.AreEqual(typeof(TestFieldDecoder), actual[1].Value);
        }

        [TestMethod]
        public void RuleStore_Rules_should_be_registered()
        {
            var assemblyWithRules = Path.Combine(_testDirectory, "DataProcessor.Rules.dll");
            StoreManager.RegisterObjectsFromAssembly(assemblyWithRules);

            var actual = StoreManager.RuleStore.GetRegisteredObjects().ToList();

            var objectRegistry = new Rules.ObjectRegistry();
            var numberOfRulesRegisteredInTheDecoderRegistry = objectRegistry.GetRegisteredFieldRules().Count();

            Assert.AreEqual(numberOfRulesRegisteredInTheDecoderRegistry, actual.Count);

            Assert.AreEqual("MinDateFieldRule", actual[0].Key);
            Assert.AreEqual(typeof(Rules.MinDateFieldRule), actual[0].Value);

            Assert.AreEqual("MaxDateFieldRule", actual[1].Key);
            Assert.AreEqual(typeof(Rules.MaxDateFieldRule), actual[1].Value);

            Assert.AreEqual("MinNumberFieldRule", actual[2].Key);
            Assert.AreEqual(typeof(Rules.MinNumberFieldRule), actual[2].Value);

            Assert.AreEqual("MaxNumberFieldRule", actual[3].Key);
            Assert.AreEqual(typeof(Rules.MaxNumberFieldRule), actual[3].Value);
        }
    }

    public class TestDecoderRegistry : BaseObjectRegistry
    {
        public static IEnumerable<KeyValuePair<string, Type>> RegisteredFieldDecoders { get; } = new KeyValuePair<string, Type>[]
        {
            new KeyValuePair<string, Type>("Test-FieldA", typeof(TestFieldDecoder)),
            new KeyValuePair<string, Type>("Test-FieldB", typeof(TestFieldDecoder))
        };

        public override IEnumerable<KeyValuePair<string, Type>> GetRegisteredFieldDecoders() => RegisteredFieldDecoders;
    }

    public class TestFieldDecoder : Decoders.FieldDecoder
    {
        public override void Decode(Field field)
        {
            field.ValidationResult = ValidationResultType.Valid;
            field.Value = field.Raw;
        }
    }
}
