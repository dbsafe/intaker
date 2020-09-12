using DataProcessor.Domain.Models;
using DataProcessor.Domain.Utils;
using DataProcessor.InputDefinitionFile;
using DataProcessor.ObjectStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessorTest_Integration_Rules
    {
        private ProcessorDefinition.Models.FileProcessorDefinition _fileProcessorDefinition;
        private readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DataProcessorGlobal.IsDebugEnabled = true;
            var assemblyWithDecoders = Path.Combine(_testDirectory, "DataProcessor.Decoders.dll");
            StoreManager.RegisterObjectsFromAssembly(assemblyWithDecoders);
            assemblyWithDecoders = Path.Combine(_testDirectory, "DataProcessor.Rules.dll");
            StoreManager.RegisterObjectsFromAssembly(assemblyWithDecoders);
            assemblyWithDecoders = Path.Combine(_testDirectory, "DataProcessor.Aggregators.dll");
            StoreManager.RegisterObjectsFromAssembly(assemblyWithDecoders);

            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer.definition.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile_10>(path);
            _fileProcessorDefinition = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);
        }

        [TestMethod]
        public void Process_Given_a_rule_violation_Result_should_indicate_the_error()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource("balance-with-rule-violations.csv", false);
            var target = new ParsedDataProcessor(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.Errors);
            TestContext.PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.InvalidCritical, actual.ValidationResult);
            Assert.AreEqual(2, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(2, actual.InvalidRows.Count);

            Assert.AreEqual("Header row is not valid", actual.Errors[0]);
            Assert.AreEqual("Trailer row is not valid", actual.Errors[1]);

            Assert.AreSame(actual.Header, actual.InvalidRows[0]);
            Assert.AreEqual(ValidationResultType.InvalidCritical, actual.Header.ValidationResult);
            Assert.AreEqual(ValidationResultType.InvalidCritical, actual.Header.Fields[3].ValidationResult);
            Assert.AreEqual(1, actual.Header.Errors.Count);
            Assert.AreEqual("Sequence number should be equal or less than 100", actual.Header.Errors[0]);

            Assert.AreSame(actual.Trailer, actual.InvalidRows[1]);
            Assert.AreEqual(ValidationResultType.InvalidCritical, actual.Trailer.ValidationResult);
            Assert.AreEqual(ValidationResultType.InvalidWarning, actual.Trailer.Fields[1].ValidationResult);
            Assert.AreEqual(ValidationResultType.InvalidCritical, actual.Trailer.Fields[2].ValidationResult);
            Assert.AreEqual(2, actual.Trailer.Errors.Count);
            Assert.AreEqual("Balance Total is incorrect", actual.Trailer.Errors[0]);
            Assert.AreEqual("Record Count should match the number data row", actual.Trailer.Errors[1]);
        }
    }
}
