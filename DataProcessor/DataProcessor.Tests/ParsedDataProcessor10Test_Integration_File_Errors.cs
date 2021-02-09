using DataProcessor.InputDefinitionFile;
using DataProcessor.Models;
using DataProcessor.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessor10Test_Integration_File_Errors
    {
        private ProcessorDefinition.Models.FileProcessorDefinition10 _fileProcessorDefinition;
        private readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DataProcessorGlobal.IsDebugEnabled = true;
            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer.definition.10.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile10>(path);
            _fileProcessorDefinition = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);
        }

        [TestMethod]
        public void Process_Given_a_file_with_a_missing_header_Should_indicate_the_error()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource<ParserContext>("balance-missing-header.10.csv", false);
            var target = new ParsedDataProcessor10(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(2, actual.Errors.Count);
            Assert.AreEqual(4, actual.AllRows.Count);
            Assert.AreEqual(2, actual.DataRows.Count);
            Assert.AreEqual(2, actual.InvalidRows.Count);

            Assert.AreEqual("Header row is invalid", actual.Errors[0]);
            Assert.AreEqual("Trailer row is invalid", actual.Errors[1]);

            Assert.IsNotNull(actual.Header);
            Assert.AreEqual(ValidationResultType.Error, actual.Header.ValidationResult);
            Assert.AreEqual(1, actual.Header.Errors.Count);
            Assert.AreEqual("Header Row - The expected number of fields 4 is not equal to the actual number of fields 8", actual.Header.Errors[0]);
            Assert.AreEqual(0, actual.Header.Warnings.Count);

            Assert.IsNotNull(actual.Trailer);
            Assert.AreEqual(ValidationResultType.Error, actual.Trailer.ValidationResult);
            Assert.AreEqual(1, actual.Trailer.Errors.Count);
            Assert.AreEqual("Record Count should match the number data row", actual.Trailer.Errors[0]);
        }

        [TestMethod]
        public void Process_Given_a_decoder_with_critical_validation_result_Should_abort_the_process()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource<ParserContext>("balance-with-invalid-header.10.csv", false);

            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer-with-critical-decoder.definition.10.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile10>(path);
            var fileProcessorDefinitionWithCritical = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            var target = new ParsedDataProcessor10(fileDataSourceValidFile, fileProcessorDefinitionWithCritical);

            var actual = target.Process();
            TestContext.PrintJson(actual);

            Assert.AreEqual(ValidationResultType.Critical, actual.ValidationResult);
            Assert.AreEqual(1, actual.Errors.Count);
            Assert.AreEqual(1, actual.AllRows.Count);
            Assert.AreEqual(0, actual.DataRows.Count);
            Assert.AreEqual(1, actual.InvalidRows.Count);

            Assert.AreEqual("Header row is invalid", actual.Errors[0]);

            Assert.IsNotNull(actual.Header);
            Assert.AreEqual(ValidationResultType.Critical, actual.Header.ValidationResult);
            Assert.AreEqual(1, actual.Header.Errors.Count);
            Assert.AreEqual("Invalid Record Type (Header Row) 'H'", actual.Header.Errors[0]);
            Assert.AreEqual(0, actual.Header.Warnings.Count);

            Assert.IsNull(actual.Trailer);
        }
    }
}
