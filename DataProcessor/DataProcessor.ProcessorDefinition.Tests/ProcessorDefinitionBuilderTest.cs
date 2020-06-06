using DataProcessor.Domain.Utils;
using DataProcessor.InputDefinitionFile;
using DataProcessor.ObjectStore;
using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataProcessor.ProcessorDefinition.Tests
{
    [TestClass]
    public class ProcessorDefinitionBuilderTest
    {
        private string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DataProcessorGlobal.IsDebugEnabled = true;
            
            var assemblyWithDecoders = Path.Combine(_testDirectory, "DataProcessor.Decoders.dll");
            StoreManager.RegisterObjectsFromAssembly(assemblyWithDecoders);
            
            PrintLoadedAssemblies();
        }

        [TestMethod]
        public void CreateProcessorDefinition_Gien_an_input_definition_file_Decoders_should_be_created()
        {
            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer.definition.xml");

            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile_10>(path);

            var actual = ProcessorDefinitionBuilder.CreateProcessorDefinition(inputDefinitionFile);

            Assert.IsNotNull(actual.HeaderRowProcessorDefinition);
            Assert.IsNotNull(actual.DataRowProcessorDefinition);
            Assert.IsNotNull(actual.TrailerRowProcessorDefinition);

            Assert.Inconclusive();
        }

        private void PrintLoadedAssemblies()
        {
            var sb = new StringBuilder();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var count = 0;
            sb.AppendSafeWithNewLine($"{assemblies.Count()} Loaded Assemblies");

            foreach (var assemblie in assemblies)
            {
                sb.AppendSafeWithNewLine($"[{count++}] FullName: {assemblie.FullName}, IsDynamic: {assemblie.IsDynamic}");
                if (!assemblie.IsDynamic)
                {
                    sb.AppendSafeWithNewLine($"Location: {assemblie.Location}");
                }

                sb.AppendSafeWithNewLine(string.Empty);
            }

            Print(sb.ToString());
        }

        private void Print(string message)
        {
            TestContext.WriteLine(message);
        }
    }
}
