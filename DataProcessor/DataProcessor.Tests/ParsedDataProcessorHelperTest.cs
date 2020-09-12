using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessorHelperTest
    {
        [TestMethod]
        [DataRow(ValidationResultType.Valid, ValidationResultType.Valid, ValidationResultType.Valid)]
        [DataRow(ValidationResultType.Valid, ValidationResultType.InvalidWarning, ValidationResultType.InvalidWarning)]
        [DataRow(ValidationResultType.Valid, ValidationResultType.InvalidCritical, ValidationResultType.InvalidCritical)]

        [DataRow(ValidationResultType.InvalidWarning, ValidationResultType.Valid, ValidationResultType.InvalidWarning)]
        [DataRow(ValidationResultType.InvalidWarning, ValidationResultType.InvalidWarning, ValidationResultType.InvalidWarning)]
        [DataRow(ValidationResultType.InvalidWarning, ValidationResultType.InvalidCritical, ValidationResultType.InvalidCritical)]

        [DataRow(ValidationResultType.InvalidCritical, ValidationResultType.Valid, ValidationResultType.InvalidCritical)]
        [DataRow(ValidationResultType.InvalidCritical, ValidationResultType.InvalidWarning, ValidationResultType.InvalidCritical)]
        [DataRow(ValidationResultType.InvalidCritical, ValidationResultType.InvalidCritical, ValidationResultType.InvalidCritical)]
        public void GetMaxValidationResult(ValidationResultType value1, ValidationResultType value2, ValidationResultType expected)
        {
            var actual = ParsedDataProcessorHelper.GetMaxValidationResult(value1, value2);
            Assert.AreEqual(expected, actual);
        }
    }
}
