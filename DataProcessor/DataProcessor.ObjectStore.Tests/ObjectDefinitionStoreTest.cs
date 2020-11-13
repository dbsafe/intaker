using DataProcessor.Decoders;
using DataProcessor.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DataProcessor.ObjectStore.Tests
{
    [TestClass]
    public class ObjectDefinitionStoreTest
    {
        private ObjectDefinitionStore<IFieldDecoder> _target;

        [TestInitialize]
        public void Initialize()
        {
            _target = new ObjectDefinitionStore<IFieldDecoder>("FieldDecoder");
        }

        [TestMethod]
        public void Register_Given_a_decoder_It_can_be_found_after_registered()
        {
            _target.Register("decoder-1", typeof(DateDecoder));

            var actual = _target.GetRegisteredObjects();
            var decodersFound = actual.Select(a => a.Key == "decoder-1" && a.Value == typeof(DateDecoder));

            Assert.AreEqual(1, decodersFound.Count());
        }

        [TestMethod]
        public void Register_Given_a_decoder_that_does_not_implement_the_correct_interface_Should_throw_an_exception()
        {
            try
            {
                _target.Register("decoder-1", typeof(DateTime));
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("FieldDecoder - Type: 'DateTime' does not implement 'IFieldDecoder'", ex.Message);
                return;
            }

            Assert.Fail("An InvalidOperationException was not thrown");
        }

        [TestMethod]
        public void Register_Given_a_decoder_that_is_already_registered_Should_throw_an_exception()
        {
            _target.Register("decoder-1", typeof(DateDecoder));

            try
            {
                _target.Register("decoder-1", typeof(DateDecoder));
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("FieldDecoder - A registered FieldDecoder with the name 'decoder-1' already exists", ex.Message);
                return;
            }

            Assert.Fail("An InvalidOperationException was not thrown");
        }

        [TestMethod]
        public void CreateDecoder_Given_a_registered_decoder_Should_return_an_instance()
        {
            _target.Register("decoder-1", typeof(DateDecoder));

            var actual = _target.CreateObject("decoder-1");
            Assert.AreEqual(typeof(DateDecoder), actual.GetType());
        }

        [TestMethod]
        public void CreateDecoder_Given_a_unregistered_decoder_Should_throw_an_exception()
        {
            _target.Register("decoder-1", typeof(DateDecoder));

            try
            {
                var actual = _target.CreateObject("decoder-2");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("FieldDecoder - 'decoder-2' not found", ex.Message);
                return;
            }

            Assert.Fail("An InvalidOperationException was not thrown");
        }
    }
}
