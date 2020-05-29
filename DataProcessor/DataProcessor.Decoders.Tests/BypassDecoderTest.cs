using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.Decoders.Tests
{
    [TestClass]
    public class BypassDecoderTest
    {
        [TestMethod]
        public void Decode_Given_a_field_ValidateResult_should_be_set_to_valid()
        {
            var field = new Field { Raw = "abc" };
            var target = new BypassDecoder();

            target.Decode(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Decode_Given_a_field_Value_should_not_be_modified()
        {
            var field = new Field { Raw = "abc", Value = "123" };
            var target = new BypassDecoder();

            target.Decode(field);

            Assert.AreEqual("123", field.Value);
        }
    }
}
