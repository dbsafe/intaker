using DataProcessor.Decoders;
using DataProcessor.Domain.Utils;
using DataProcessor.InputDefinitionFile;
using DataProcessor.ObjectStore;
using DataProcessor.ProcessorDefinition.Models;
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
            Assert.IsNotNull(actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(5, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions.Length);

            AssertFieldProcessorDefinition("RecordType", "HEADER", typeof(TextDecoder), actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[0]);
            AssertFieldProcessorDefinition("CreationDate", "MMddyyyy", typeof(DateDecoder), actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[1]);
            AssertFieldProcessorDefinition("LocationID", "[a-zA-Z]{12}", typeof(TextDecoder), actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[2]);
            AssertFieldProcessorDefinition("SequenceNumber", "(?!0{4})[0-9]{4}", typeof(NumberDecoder), actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[3]);
            AssertFieldProcessorDefinition("Optional", null, typeof(BypassDecoder), actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[4]);

            Assert.IsNotNull(actual.DataRowProcessorDefinition);
            Assert.IsNotNull(actual.DataRowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(7, actual.DataRowProcessorDefinition.FieldProcessorDefinitions.Length);

            Assert.IsNotNull(actual.TrailerRowProcessorDefinition);
            Assert.IsNotNull(actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(3, actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions.Length);
        }

        private void AssertFieldProcessorDefinition(string expectedFieldName, string expectedPattern, Type expectedType, FieldProcessorDefinition fieldProcessorDefinition)
        {
            Assert.AreEqual(expectedFieldName, fieldProcessorDefinition.FieldName);
            Assert.IsNotNull(fieldProcessorDefinition.Decoder);
            Assert.AreEqual(expectedPattern, fieldProcessorDefinition.Decoder.Pattern);
            Assert.AreEqual(expectedType, fieldProcessorDefinition.Decoder.GetType());
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
