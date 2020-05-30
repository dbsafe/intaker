using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataProcessor.Decoders.Tests
{
    [TestClass]
    public class TextDecoderTest
    {
        private TextDecoder target;

        [TestMethod]
        public void Decode_Given_that_property_pattern_is_not_set_Should_throw_an_exception()
        {
            var field = new Field { Raw = "abc" };
            target = new TextDecoder { FailValidationResult = ValidationResultType.InvalidFixable };

            try
            {
                target.Decode(field);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Property Pattern cannot be empty or null", ex.Message);
                return;
            }

            Assert.Fail("An exception was not thrown");
        }

        [TestMethod]
        public void Decode_Given_that_property_validationResult_is_not_set_Should_throw_an_exception()
        {
            var field = new Field { Raw = "AAA" };
            target = new TextDecoder { Pattern = "(AAA|BBB)" };

            try
            {
                target.Decode(field);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Property FailValidationResult cannot be empty or null", ex.Message);
                return;
            }

            Assert.Fail("An exception was not thrown");
        }

        [TestMethod]
        public void Decode_Given_a_valid_text_Value_should_be_set_with_the_text()
        {
            var field = new Field { Raw = "BBB" };
            target = new TextDecoder { Pattern = "(AAA|BBB)", FailValidationResult = ValidationResultType.InvalidFixable };

            target.Decode(field);

            Assert.AreEqual("BBB", field.Value);
        }

        [TestMethod]
        public void Decode_Given_a_valid_text_ValidationResult_should_be_valid()
        {
            var field = new Field { Raw = "AAA" };
            target = new TextDecoder { Pattern = "(AAA|BBB)", FailValidationResult = ValidationResultType.InvalidFixable };

            target.Decode(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Decode_Given_an_invalid_text_Value_should_be_null()
        {
            var field = new Field { Raw = "ABC" };
            target = new TextDecoder { Pattern = "(AAA|BBB)", FailValidationResult = ValidationResultType.InvalidFixable };

            target.Decode(field);

            Assert.IsNull(field.Value);
        }

        [TestMethod]
        [DataRow(ValidationResultType.InvalidCritical)]
        [DataRow(ValidationResultType.InvalidFixable)]
        public void Decode_Given_an_invalid_text_ValidationResult_should_be_set_with_the_value_assigned_to_the_decoder(ValidationResultType failValidationResult)
        {
            var field = new Field { Raw = "ABC" };
            target = new TextDecoder { Pattern = "(AAA|BBB)", FailValidationResult = failValidationResult };

            target.Decode(field);

            Assert.AreEqual(failValidationResult, field.ValidationResult);
        }
    }
}
