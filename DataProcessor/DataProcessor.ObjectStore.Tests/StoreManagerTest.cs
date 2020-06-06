using DataProcessor.Decoders;
using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.ObjectStore.Tests
{
    [TestClass]
    public class StoreManagerTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Domain.Utils.DataProcessorGlobal.IsDebugEnabled = true;
        }

        [TestMethod]
        public void DecoderStore_Decoders_should_be_registered()
        {
            var actual = StoreManager.DecoderStore.GetRegisteredObjects().ToList();

            var numberOfDecodersRegisteredInThisTest = TestDecoderRegistry.RegisteredDecoders.Count();
            var numberOfDecodersRegisteredInTheDecoderRegistry = ObjectRegistry.RegisteredDecoders.Count();

            Assert.AreEqual(numberOfDecodersRegisteredInThisTest + numberOfDecodersRegisteredInTheDecoderRegistry, actual.Count);

            Assert.AreEqual("Test-FieldA", actual[0].Key);
            Assert.AreEqual(typeof(TestFieldDecoder), actual[0].Value);

            Assert.AreEqual("Test-FieldB", actual[1].Key);
            Assert.AreEqual(typeof(TestFieldDecoder), actual[1].Value);
        }
    }

    public class TestDecoderRegistry : IObjectRegistry
    {
        public static IEnumerable<KeyValuePair<string, Type>> RegisteredDecoders { get; } = new KeyValuePair<string, Type>[]
        {
            new KeyValuePair<string, Type>("Test-FieldA", typeof(TestFieldDecoder)),
            new KeyValuePair<string, Type>("Test-FieldB", typeof(TestFieldDecoder))
        };

        public IEnumerable<KeyValuePair<string, Type>> GetRegisteredDecoders() => RegisteredDecoders;
    }

    public class TestFieldDecoder : FieldDecoder
    {
        public override void Decode(Field field)
        {
            field.ValidationResult = ValidationResultType.Valid;
            field.Value = field.Raw;
        }
    }
}
