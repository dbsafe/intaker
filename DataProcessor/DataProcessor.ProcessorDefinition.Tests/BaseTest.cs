using DataProcessor.Models;
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
    public class BaseTest
    {
        protected readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public TestContext TestContext { get; set; }


        protected void AssertFieldProcessorDefinition(
                    string expectedFieldName,
                    string expectedPattern,
                    Type expectedType,
                    ValidationResultType expectedFailValidationResult,
                    FieldProcessorDefinition fieldProcessorDefinition)
        {
            Assert.AreEqual(expectedFieldName, fieldProcessorDefinition.FieldName);
            Assert.IsNotNull(fieldProcessorDefinition.Decoder);
            Assert.AreEqual(expectedPattern, fieldProcessorDefinition.Decoder.Pattern);
            Assert.AreEqual(expectedType, fieldProcessorDefinition.Decoder.GetType());
            Assert.AreEqual(expectedFailValidationResult, fieldProcessorDefinition.Decoder.FailValidationResult);
        }

        private void Print(string message)
        {
            TestContext.WriteLine(message);
        }

        protected void PrintLoadedAssemblies()
        {
            var sb = new StringBuilder();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var count = 0;
            sb.AppendSafeWithNewLine($"{assemblies.Count()} Loaded Assemblies");

            foreach (var assembly in assemblies)
            {
                sb.AppendSafeWithNewLine($"[{count++}] FullName: {assembly.FullName}, IsDynamic: {assembly.IsDynamic}");
                if (!assembly.IsDynamic)
                {
                    sb.AppendSafeWithNewLine($"Location: {assembly.Location}");
                }

                sb.AppendSafeWithNewLine(string.Empty);
            }

            Print(sb.ToString());
        }
    }
}
