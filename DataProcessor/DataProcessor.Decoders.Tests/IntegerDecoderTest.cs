using DataProcessor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataProcessor.Decoders.Tests
{

    [TestClass]
    public class IntegerDecoderTest
    {
        private IntegerDecoder target;

        [TestMethod]
        public void Decode_Given_that_property_pattern_is_not_set_Should_throw_an_exception()
        {
            var field = new Field { Raw = "10" };
            target = new IntegerDecoder { FailValidationResult = ValidationResultType.Warning };

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
        public void Decode_Given_a_valid_number_Value_should_be_set_with_the_number()
        {
            var field = new Field { Raw = "10" };
            target = new IntegerDecoder { Pattern = @"(?!0+)-?[0-9]{1,6}", FailValidationResult = ValidationResultType.Warning };

            target.Decode(field);

            Assert.AreEqual(10, field.Value);
        }

        [TestMethod]
        public void Decode_Given_a_valid_number_ValidationResult_should_be_valid()
        {
            var field = new Field { Raw = "10" };
            target = new IntegerDecoder { Pattern = @"(?!0+)-?[0-9]{1,6}", FailValidationResult = ValidationResultType.Warning };

            target.Decode(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Decode_Given_an_invalid_number_Value_should_be_null()
        {
            var field = new Field { Raw = "10a" };
            target = new IntegerDecoder { Pattern = @"(?!0+)-?[0-9]{1,6}", FailValidationResult = ValidationResultType.Warning };

            target.Decode(field);

            Assert.IsNull(field.Value);
        }

        [TestMethod]
        [DataRow(ValidationResultType.Critical)]
        [DataRow(ValidationResultType.Warning)]
        public void Decode_Given_an_invalid_number_ValidationResult_should_be_set_with_the_value_assigned_to_the_decoder(ValidationResultType failValidationResult)
        {
            var field = new Field { Raw = "10a" };
            target = new IntegerDecoder { Pattern = @"(?!0+)-?[0-9]{1,6}", FailValidationResult = failValidationResult };

            target.Decode(field);

            Assert.AreEqual(failValidationResult, field.ValidationResult);
        }
    }
}
