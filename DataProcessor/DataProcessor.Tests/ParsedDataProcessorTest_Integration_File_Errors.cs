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
    public class ParsedDataProcessorTest_Integration_File_Errors
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
            
            var assemblyWithRules = Path.Combine(_testDirectory, "DataProcessor.Rules.dll");
            StoreManager.RegisterObjectsFromAssembly(assemblyWithRules);

            var assemblyWithAggregators = Path.Combine(_testDirectory, "DataProcessor.Aggregators.dll");
            StoreManager.RegisterObjectsFromAssembly(assemblyWithAggregators);

            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer.definition.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile_10>(path);
            _fileProcessorDefinition = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);
        }

        [TestMethod]
        public void Process_Given_a_file_with_a_missing_header_Should_indicate_the_error()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource("balance-missing-header.csv", false);
            var target = new ParsedDataProcessor(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.Errors);
            TestContext.PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.Critical, actual.ValidationResult);
            Assert.AreEqual(2, actual.Errors.Count);
            Assert.AreEqual(4, actual.AllRows.Count);
            Assert.AreEqual(2, actual.DataRows.Count);
            Assert.AreEqual(2, actual.InvalidRows.Count);

            Assert.AreEqual("Header row is not valid", actual.Errors[0]);
            Assert.AreEqual("Trailer row is not valid", actual.Errors[1]);
        }
    }
}
