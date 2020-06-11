using DataProcessor.Domain.Models;
using DataProcessor.Domain.Utils;
using DataProcessor.InputDefinitionFile;
using DataProcessor.ObjectStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessorTest_Integration_Decoders
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
        public void Process_Given_a_valid_file_There_should_no_be_errors()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource("balance-with-header-and-trailer.csv", false);
            var target = new ParsedDataProcessor(fileDataSourceValidFile, _processorDefinition);

            var actual = target.Process();
            PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.Valid, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);
        }

        [TestMethod]
        public void Process_Given_a_valid_file_Header_should_be_decoded()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource("balance-with-header-and-trailer.csv", false);
            var target = new ParsedDataProcessor(fileDataSourceValidFile, _processorDefinition);

            var actual = target.Process();
            PrintJson(actual.AllRows);

            Assert.IsNotNull(actual.Header);
            Assert.AreEqual(0, actual.Header.Index);
            Assert.AreEqual(ValidationResultType.Valid, actual.Header.ValidationResult);
            Assert.AreEqual("HEADER,09212013,ABCDCompLndn,0001", actual.Header.Raw);
            Assert.AreEqual("HEADER,09212013,ABCDCompLndn,0001", string.Join(',', actual.Header.RawFields));
            Assert.AreEqual(0, actual.Header.Errors.Count);

            Assert.AreEqual(4, actual.Header.Fields.Count);

            AssertValidField(0, "HEADER", "HEADER", actual.Header, actual.Header.Fields[0]);
            AssertValidField(1, "09212013", new DateTime(2013, 9, 21), actual.Header, actual.Header.Fields[1]);
            AssertValidField(2, "ABCDCompLndn", "ABCDCompLndn", actual.Header, actual.Header.Fields[2]);
            AssertValidField(3, "0001", 1m, actual.Header, actual.Header.Fields[3]);
        }

        [TestMethod]
        public void Process_Given_a_valid_file_Data_rows_should_be_decoded()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource("balance-with-header-and-trailer.csv", false);
            var target = new ParsedDataProcessor(fileDataSourceValidFile, _processorDefinition);

            var actual = target.Process();
            PrintJson(actual.AllRows);

            var dataRow0 = actual.DataRows[0];
            Assert.AreEqual(1, dataRow0.Index);
            Assert.AreEqual(ValidationResultType.Valid, dataRow0.ValidationResult);
            Assert.AreEqual("BALANCE,1001,111-22-1001,fname-01,lname-01,10212000,1000.00,AA", dataRow0.Raw);
            Assert.AreEqual("BALANCE,1001,111-22-1001,fname-01,lname-01,10212000,1000.00,AA", string.Join(',', dataRow0.RawFields));
            Assert.AreEqual(0, dataRow0.Errors.Count);

            Assert.AreEqual(8, dataRow0.Fields.Count);

            AssertValidField(0, "BALANCE", "BALANCE", dataRow0, dataRow0.Fields[0]);
            AssertValidField(1, "1001", 1001m, dataRow0, dataRow0.Fields[1]);
            AssertValidField(2, "111-22-1001", "111-22-1001", dataRow0, dataRow0.Fields[2]);
            AssertValidField(3, "fname-01", "fname-01", dataRow0, dataRow0.Fields[3]);
            AssertValidField(4, "lname-01", "lname-01", dataRow0, dataRow0.Fields[4]);
            AssertValidField(5, "10212000", new DateTime(2000, 10, 21), dataRow0, dataRow0.Fields[5]);
            AssertValidField(6, "1000.00", 1000m, dataRow0, dataRow0.Fields[6]);
            AssertValidField(7, "AA", "AA", dataRow0, dataRow0.Fields[7]);

            var dataRow1 = actual.DataRows[1];
            Assert.AreEqual(2, dataRow1.Index);
            Assert.AreEqual(ValidationResultType.Valid, dataRow1.ValidationResult);
            Assert.AreEqual("BALANCE,1002,111-22-1002,fname-02,lname-02,10222000,2000.00,", dataRow1.Raw);
            Assert.AreEqual("BALANCE,1002,111-22-1002,fname-02,lname-02,10222000,2000.00,", string.Join(',', dataRow1.RawFields));
            Assert.AreEqual(0, dataRow1.Errors.Count);

            Assert.AreEqual(8, dataRow1.Fields.Count);

            AssertValidField(0, "BALANCE", "BALANCE", dataRow1, dataRow1.Fields[0]);
            AssertValidField(1, "1002", 1002m, dataRow1, dataRow1.Fields[1]);
            AssertValidField(2, "111-22-1002", "111-22-1002", dataRow1, dataRow1.Fields[2]);
            AssertValidField(3, "fname-02", "fname-02", dataRow1, dataRow1.Fields[3]);
            AssertValidField(4, "lname-02", "lname-02", dataRow1, dataRow1.Fields[4]);
            AssertValidField(5, "10222000", new DateTime(2000, 10, 22), dataRow1, dataRow1.Fields[5]);
            AssertValidField(6, "2000.00", 2000m, dataRow1, dataRow1.Fields[6]);
            AssertValidField(7, "", "", dataRow1, dataRow1.Fields[7]);

            var dataRow2 = actual.DataRows[2];
            Assert.AreEqual(3, dataRow2.Index);
            Assert.AreEqual(ValidationResultType.Valid, dataRow2.ValidationResult);
            Assert.AreEqual("BALANCE,1003,111-22-1003,fname-03,lname-03,10232000,3000.00,", dataRow2.Raw);
            Assert.AreEqual("BALANCE,1003,111-22-1003,fname-03,lname-03,10232000,3000.00,", string.Join(',', dataRow2.RawFields));
            Assert.AreEqual(0, dataRow2.Errors.Count);

            Assert.AreEqual(8, dataRow2.Fields.Count);

            AssertValidField(0, "BALANCE", "BALANCE", dataRow2, dataRow2.Fields[0]);
            AssertValidField(1, "1003", 1003m, dataRow2, dataRow2.Fields[1]);
            AssertValidField(2, "111-22-1003", "111-22-1003", dataRow2, dataRow2.Fields[2]);
            AssertValidField(3, "fname-03", "fname-03", dataRow2, dataRow2.Fields[3]);
            AssertValidField(4, "lname-03", "lname-03", dataRow2, dataRow2.Fields[4]);
            AssertValidField(5, "10232000", new DateTime(2000, 10, 23), dataRow2, dataRow2.Fields[5]);
            AssertValidField(6, "3000.00", 3000m, dataRow2, dataRow2.Fields[6]);
            AssertValidField(7, "", "", dataRow2, dataRow2.Fields[7]);
        }

        [TestMethod]
        public void Process_Given_a_valid_file_Trailer_should_be_decoded()
        {
            var fileDataSourceValidFile = TestHelpers.CreateFileDataSource("balance-with-header-and-trailer.csv", false);
            var target = new ParsedDataProcessor(fileDataSourceValidFile, _processorDefinition);

            var actual = target.Process();
            PrintJson(actual.AllRows);

            Assert.IsNotNull(actual.Trailer);
            Assert.AreEqual(4, actual.Trailer.Index);
            Assert.AreEqual(ValidationResultType.Valid, actual.Trailer.ValidationResult);
            Assert.AreEqual("TRAILER,6000.00,3", actual.Trailer.Raw);
            Assert.AreEqual("TRAILER,6000.00,3", string.Join(',', actual.Trailer.RawFields));
            Assert.AreEqual(0, actual.Trailer.Errors.Count);

            Assert.AreEqual(3, actual.Trailer.Fields.Count);

            AssertValidField(0, "TRAILER", "TRAILER", actual.Trailer, actual.Trailer.Fields[0]);
            AssertValidField(1, "6000.00", 6000m, actual.Trailer, actual.Trailer.Fields[1]);
            AssertValidField(2, "3", 3m, actual.Trailer, actual.Trailer.Fields[2]);
        }

        [TestMethod]
        public void Process_Given_an_invalid_header_Should_indicate_error()
        {
            var fileDataSourceInvalidHeader = TestHelpers.CreateFileDataSource("balance-with-invalid-header.csv", false);
            var target = new ParsedDataProcessor(fileDataSourceInvalidHeader, _processorDefinition);

            var actual = target.Process();
            PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.InvalidCritical, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(1, actual.InvalidRows.Count);

            Assert.AreSame(actual.Header, actual.InvalidRows[0]);
            var invalidRow = actual.Header;
            Assert.AreEqual(ValidationResultType.InvalidCritical, invalidRow.ValidationResult);
            Assert.AreEqual(1, invalidRow.Errors.Count);
            Assert.AreEqual("Invalid RecordType 'H'", invalidRow.Errors[0]);
        }

        [TestMethod]
        public void Process_Given_an_invalid_data_row_Should_indicate_error()
        {
            var fileDataSourceInvalidHeader = TestHelpers.CreateFileDataSource("balance-with-invalid-date-in-a-data-row.csv", false);
            var target = new ParsedDataProcessor(fileDataSourceInvalidHeader, _processorDefinition);

            var actual = target.Process();
            PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.InvalidFixable, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(1, actual.InvalidRows.Count);

            Assert.AreSame(actual.AllRows[2], actual.InvalidRows[0]);
            
            var invalidRow = actual.AllRows[2];
            Assert.AreEqual(ValidationResultType.InvalidFixable, invalidRow.ValidationResult);
            Assert.AreEqual(1, invalidRow.Errors.Count);
            Assert.AreEqual("Invalid DOB '1022200a'", invalidRow.Errors[0]);
        }

        [TestMethod]
        public void Process_Given_an_invalid_trailer_Should_indicate_error()
        {
            var fileDataSourceInvalidHeader = TestHelpers.CreateFileDataSource("balance-with-invalid-trailer.csv", false);
            var target = new ParsedDataProcessor(fileDataSourceInvalidHeader, _processorDefinition);

            var actual = target.Process();
            PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.InvalidCritical, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(1, actual.InvalidRows.Count);

            Assert.AreSame(actual.Trailer, actual.InvalidRows[0]);
            var invalidRow = actual.Trailer;
            Assert.AreEqual(ValidationResultType.InvalidCritical, invalidRow.ValidationResult);
            Assert.AreEqual(1, invalidRow.Errors.Count);
            Assert.AreEqual("Invalid BalanceTotal '6000.oo'", invalidRow.Errors[0]);
        }

        private void PrintJson(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            Print(json);
        }

        private void Print(string message)
        {
            TestContext.WriteLine(message);
        }

        public void AssertValidField(int expectedIndex, string expectedRaw, object expectedValue, Row expectedRow, Field actualField)
        {
            Assert.AreEqual(expectedIndex, actualField.Index);
            Assert.AreEqual(expectedRaw, actualField.Raw);
            Assert.AreEqual(expectedValue, actualField.Value);
            Assert.AreEqual(ValidationResultType.Valid, actualField.ValidationResult);
            Assert.AreSame(expectedRow, actualField.Row);
        }
    }
}
