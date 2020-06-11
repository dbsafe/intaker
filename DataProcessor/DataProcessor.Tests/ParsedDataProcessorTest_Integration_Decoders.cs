using DataProcessor.DataSource.File;
using DataProcessor.Domain.Utils;
using DataProcessor.InputDefinitionFile;
using DataProcessor.ObjectStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessorTest_Integration_Decoders
    {
        private ProcessorDefinition.Models.ProcessorDefinition _processorDefinition;
        private FileDataSource _fileDataSource;
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

            _fileDataSource = TestHelpers.CreateFileDataSource("balance-with-header-and-trailer.csv", false);
            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer.definition.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile_10>(path);
            _processorDefinition = ProcessorDefinition.ProcessorDefinitionBuilder.CreateProcessorDefinition(inputDefinitionFile);
        }

        [TestMethod]
        public void Process_Given_a_valid_file_Fields_should_be_decoded()
        {
            var target = new ParsedDataProcessor(_fileDataSource, _processorDefinition);

            var actual = target.Process();

            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);

        }
    }
}
