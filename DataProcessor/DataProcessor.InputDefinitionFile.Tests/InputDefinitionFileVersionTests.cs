using DataProcessor.InputDefinitionFile.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace DataProcessor.InputDefinitionFile.Tests
{
    [TestClass]
    public class InputDefinitionFileVersionTests
    {
        private readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Deserialize_v10()
        {
            var path = Path.Combine(_testDirectory, "TestFiles", "FXWDCSV.definition.10.xml");
            var inputXml = File.ReadAllText(path);

            var actual = HelperXmlSerializer.Deserialize<InputDefinitionFrameworkVersion>(inputXml);

            Assert.AreEqual("1.0", actual.FrameworkVersion);
        }

        [TestMethod]
        public void Deserialize_v20()
        {
            var path = Path.Combine(_testDirectory, "TestFiles", "FXWDCSV.definition.20.xml");
            var inputXml = File.ReadAllText(path);

            var actual = HelperXmlSerializer.Deserialize<InputDefinitionFrameworkVersion>(inputXml);

            Assert.AreEqual("2.0", actual.FrameworkVersion);
        }
    }
}
