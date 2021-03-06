﻿using DataProcessor.InputDefinitionFile;
using DataProcessor.Models;
using DataProcessor.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessor20Test_Integration_Rules
    {
        private ProcessorDefinition.Models.FileProcessorDefinition20 _fileProcessorDefinition;
        private readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DataProcessorGlobal.IsDebugEnabled = true;
            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer.definition.20.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile20>(path);
            _fileProcessorDefinition = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);
        }

        [TestMethod]
        public void Process_Given_a_rule_violation_in_the_header_Result_should_indicate_the_error()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource<ParserContext20>("balance-with-rule-violations.20.csv", false);
            var target = new ParsedDataProcessor20(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(5, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);

            Assert.AreEqual(2, actual.Errors.Count);
            Assert.AreEqual("Header row is invalid", actual.Errors[0]);

            Assert.AreEqual(ValidationResultType.Error, actual.Header.ValidationResult);
            Assert.AreEqual(ValidationResultType.Error, actual.Header.Fields[3].ValidationResult);
            Assert.AreEqual(1, actual.Header.Errors.Count);
            Assert.AreEqual("Sequence number should be equal or less than 100", actual.Header.Errors[0]);
            Assert.AreEqual(1, actual.Header.Warnings.Count);
            Assert.AreEqual("Sequence number should be between 1 and 100", actual.Header.Warnings[0]);
        }

        [TestMethod]
        public void Process_Given_a_rule_violation_in_the_trailer_Result_should_indicate_error_and_warning()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource<ParserContext20>("balance-with-rule-violations.20.csv", false);
            var target = new ParsedDataProcessor20(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(5, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);

            Assert.AreEqual(2, actual.Errors.Count);
            Assert.AreEqual("Trailer row is invalid", actual.Errors[1]);

            Assert.AreEqual(ValidationResultType.Error, actual.Trailer.ValidationResult);
            Assert.AreEqual(ValidationResultType.Warning, actual.Trailer.Fields[1].ValidationResult);
            Assert.AreEqual(ValidationResultType.Error, actual.Trailer.Fields[2].ValidationResult);
            Assert.AreEqual(1, actual.Trailer.Errors.Count);
            Assert.AreEqual("Record Count should match the number data row", actual.Trailer.Errors[0]);
            Assert.AreEqual(1, actual.Trailer.Warnings.Count);
            Assert.AreEqual("Balance Total is incorrect", actual.Trailer.Warnings[0]);
        }
    }
}
