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
        private ProcessorDefinition.Models.ProcessorDefinition _processorDefinition;
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
            _processorDefinition = ProcessorDefinition.ProcessorDefinitionBuilder.CreateProcessorDefinition(inputDefinitionFile);
        }

        [TestMethod]
        public void Process_Given_a_rule_violation_Result_should_indicate_the_error()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource("balance-with-rule-violations.csv", false);
            var target = new ParsedDataProcessor(fileDataSourceValidFile, _processorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.InvalidCritical, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(1, actual.InvalidRows.Count);

            Assert.AreSame(actual.Header, actual.InvalidRows[0]);
            Assert.AreEqual(ValidationResultType.InvalidCritical, actual.Header.ValidationResult);
            Assert.AreEqual(ValidationResultType.InvalidCritical, actual.Header.Fields[3].ValidationResult);
            Assert.AreEqual(1, actual.Header.Errors.Count);
            Assert.AreEqual("Sequence number should be equal or less than 100", actual.Header.Errors[0]);
        }
    }
}
