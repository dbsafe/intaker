using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataProcessor.Decoders.Tests
{
    [TestClass]
    public class DateDecoderTest
    {
        [TestMethod]
        public void Decode_Given_that_property_pattern_is_not_set_Should_throw_an_exception()
        {
            var field = new Field { Raw = "2020-10-20" };
            var target = new DateDecoder { FailValidationResult = ValidationResultType.Warning };

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
        public void Decode_Given_that_property_failValidationResult_is_not_set_Should_throw_an_exception()
        {
            var field = new Field { Raw = "2020-10-aa", ValidationResult = ValidationResultType.Valid };
            var target = new DateDecoder { Pattern = "yyyy-MM-dd" };

            try
            {
                target.Decode(field);
                Assert.Fail("An exception was not thrown");
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("Property FailValidationResult must be set", e.Message);
            }            
        }

        [TestMethod]
        public void Decode_Given_a_valid_date_Value_should_be_set_with_the_date()
        {
            var field = new Field { Raw = "2020-10-20" };
            var target = new DateDecoder { Pattern = "yyyy-MM-dd", FailValidationResult = ValidationResultType.Warning };

            target.Decode(field);

            Assert.AreEqual(new DateTime(2020, 10, 20), field.Value);
        }

        [TestMethod]
        public void Decode_Given_a_valid_date_ValidationResult_should_be_valid()
        {
            var field = new Field { Raw = "2020-10-20" };
            var target = new DateDecoder { Pattern = "yyyy-MM-dd", FailValidationResult = ValidationResultType.Warning };

            target.Decode(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Decode_Given_an_invalid_date_Value_should_be_null()
        {
            var field = new Field { Raw = "2020-10-50" };
            var target = new DateDecoder { Pattern = "yyyy-MM-dd", FailValidationResult = ValidationResultType.Warning };

            target.Decode(field);

            Assert.IsNull(field.Value);
        }

        [TestMethod]
        [DataRow(ValidationResultType.Critical)]
        [DataRow(ValidationResultType.Warning)]
        public void Decode_Given_an_invalid_date_ValidationResult_should_be_set_with_the_value_assigned_to_the_decoder(ValidationResultType failValidationResult)
        {
            var field = new Field { Raw = "2020-10-50" };
            var target = new DateDecoder { Pattern = "yyyy-MM-dd", FailValidationResult = failValidationResult };

            target.Decode(field);

            Assert.AreEqual(failValidationResult, field.ValidationResult);
        }
    }
}
