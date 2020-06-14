using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataProcessor.Decoders.Tests
{
    [TestClass]
    public class DecimalDecoderTest
    {
        private DecimalDecoder target;

        [TestMethod]
        public void Decode_Given_that_property_pattern_is_not_set_Should_throw_an_exception()
        {
            var field = new Field { Raw = "10.05" };
            target = new DecimalDecoder { FailValidationResult = ValidationResultType.InvalidFixable };

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
            var field = new Field { Raw = "10.05" };
            target = new DecimalDecoder { Pattern = @"(?!0+)-?[0-9]{1,6}\.[0-9]{2}" };

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
        public void Decode_Given_a_valid_number_Value_should_be_set_with_the_number()
        {
            var field = new Field { Raw = "10.05" };
            target = new DecimalDecoder { Pattern = @"(?!0+)-?[0-9]{1,6}\.[0-9]{2}", FailValidationResult = ValidationResultType.InvalidFixable };

            target.Decode(field);

            Assert.AreEqual(10.05m, field.Value);
        }

        [TestMethod]
        public void Decode_Given_a_valid_number_ValidationResult_should_be_valid()
        {
            var field = new Field { Raw = "10.05" };
            target = new DecimalDecoder { Pattern = @"(?!0+)-?[0-9]{1,6}\.[0-9]{2}", FailValidationResult = ValidationResultType.InvalidFixable };

            target.Decode(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Decode_Given_an_invalid_number_Value_should_be_null()
        {
            var field = new Field { Raw = "10.ab" };
            target = new DecimalDecoder { Pattern = @"(?!0+)-?[0-9]{1,6}\.[0-9]{2}", FailValidationResult = ValidationResultType.InvalidFixable };

            target.Decode(field);

            Assert.IsNull(field.Value);
        }

        [TestMethod]
        [DataRow(ValidationResultType.InvalidCritical)]
        [DataRow(ValidationResultType.InvalidFixable)]
        public void Decode_Given_an_invalid_number_ValidationResult_should_be_set_with_the_value_assigned_to_the_decoder(ValidationResultType failValidationResult)
        {
            var field = new Field { Raw = "10.ab" };
            target = new DecimalDecoder { Pattern = @"(?!0+)-?[0-9]{1,6}\.[0-9]{2}", FailValidationResult = failValidationResult };

            target.Decode(field);

            Assert.AreEqual(failValidationResult, field.ValidationResult);
        }
    }
}
