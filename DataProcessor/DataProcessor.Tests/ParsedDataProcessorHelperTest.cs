using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessorHelperTest
    {
        [TestMethod]
        [DataRow(ValidationResultType.Valid, ValidationResultType.Valid, ValidationResultType.Valid)]
        [DataRow(ValidationResultType.Valid, ValidationResultType.Warning, ValidationResultType.Warning)]
        [DataRow(ValidationResultType.Valid, ValidationResultType.Critical, ValidationResultType.Critical)]

        [DataRow(ValidationResultType.Warning, ValidationResultType.Valid, ValidationResultType.Warning)]
        [DataRow(ValidationResultType.Warning, ValidationResultType.Warning, ValidationResultType.Warning)]
        [DataRow(ValidationResultType.Warning, ValidationResultType.Critical, ValidationResultType.Critical)]

        [DataRow(ValidationResultType.Critical, ValidationResultType.Valid, ValidationResultType.Critical)]
        [DataRow(ValidationResultType.Critical, ValidationResultType.Warning, ValidationResultType.Critical)]
        [DataRow(ValidationResultType.Critical, ValidationResultType.Critical, ValidationResultType.Critical)]
        public void GetMaxValidationResult(ValidationResultType value1, ValidationResultType value2, ValidationResultType expected)
        {
            var actual = ParsedDataProcessorHelper.GetMaxValidationResult(value1, value2);
            Assert.AreEqual(expected, actual);
        }
    }
}
