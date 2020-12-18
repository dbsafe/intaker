using DataProcessor.Contracts;
using DataProcessor.InputDefinitionFile;
using DataProcessor.Models;
using DataProcessor.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessor10Test_Integration_Decoders
    {
        private ProcessorDefinition.Models.FileProcessorDefinition10 _fileProcessorDefinition;
        private InputDefinitionFile_10 _inputDefinitionFile;
        private readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DataProcessorGlobal.IsDebugEnabled = true;
            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer.definition.10.xml");
            _inputDefinitionFile = FileLoader.Load<InputDefinitionFile_10>(path);
            _fileProcessorDefinition = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(_inputDefinitionFile);
        }

        [TestMethod]
        public void Process_Given_a_valid_file_There_should_no_be_errors()
        {
            var fileDataSourceValidFile = CreateFileDataSource("balance-with-header-and-trailer.10.csv");
            var target = new ParsedDataProcessor10(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.Valid, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);
        }

        [TestMethod]
        public void Process_Given_a_valid_file_Header_should_be_decoded()
        {
            var fileDataSourceValidFile = CreateFileDataSource("balance-with-header-and-trailer.10.csv");
            var target = new ParsedDataProcessor10(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.AllRows);
            TestContext.PrintRowJsons(actual.AllRows);

            Assert.IsNotNull(actual.Header);
            Assert.AreEqual(0, actual.Header.Index);
            Assert.AreEqual(ValidationResultType.Valid, actual.Header.ValidationResult);
            Assert.AreEqual("HEADER,09212013,ABCDCompLndn,0001", actual.Header.Raw);
            Assert.AreEqual("HEADER,09212013,ABCDCompLndn,0001", string.Join(',', actual.Header.RawFields));
            Assert.AreEqual(0, actual.Header.Errors.Count);
            Assert.AreEqual(0, actual.Header.Warnings.Count);
            Assert.AreEqual("{\"RecordType\":\"HEADER\",\"CreationDate\":\"2013-09-21T00:00:00\",\"LocationID\":\"ABCDCompLndn\",\"SequenceNumber\":1}", actual.Header.Json);

            Assert.AreEqual(4, actual.Header.Fields.Count);
            AssertValidField(0, "HEADER", "HEADER", actual.Header, actual.Header.Fields[0]);
            AssertValidField(1, "09212013", new DateTime(2013, 9, 21), actual.Header, actual.Header.Fields[1]);
            AssertValidField(2, "ABCDCompLndn", "ABCDCompLndn", actual.Header, actual.Header.Fields[2]);
            AssertValidField(3, "0001", 1, actual.Header, actual.Header.Fields[3]);
        }

        [TestMethod]
        public void Process_Given_a_valid_file_Data_rows_should_be_decoded()
        {
            var fileDataSourceValidFile = CreateFileDataSource("balance-with-header-and-trailer.10.csv");
            var target = new ParsedDataProcessor10(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.AllRows);

            var dataRow0 = actual.DataRows[0];
            Assert.AreEqual(1, dataRow0.Index);
            Assert.AreEqual(ValidationResultType.Valid, dataRow0.ValidationResult);
            Assert.AreEqual("BALANCE,1001,111-22-1001,fname-01,lname-01,10212000,1000.00,AA", dataRow0.Raw);
            Assert.AreEqual("BALANCE,1001,111-22-1001,fname-01,lname-01,10212000,1000.00,AA", string.Join(',', dataRow0.RawFields));
            Assert.AreEqual(0, dataRow0.Errors.Count);
            Assert.AreEqual(0, dataRow0.Warnings.Count);
            Assert.AreEqual("{\"RecordType\":\"BALANCE\",\"ConsumerID\":1001,\"SSN\":\"111-22-1001\",\"FirstName\":\"fname-01\",\"LastName\":\"lname-01\",\"DOB\":\"2000-10-21T00:00:00\",\"Balance\":1000.00,\"CustomField\":\"AA\"}", dataRow0.Json);

            Assert.AreEqual(8, dataRow0.Fields.Count);
            AssertValidField(0, "BALANCE", "BALANCE", dataRow0, dataRow0.Fields[0]);
            AssertValidField(1, "1001", 1001, dataRow0, dataRow0.Fields[1]);
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
            Assert.AreEqual(0, dataRow1.Warnings.Count);
            Assert.AreEqual("{\"RecordType\":\"BALANCE\",\"ConsumerID\":1002,\"SSN\":\"111-22-1002\",\"FirstName\":\"fname-02\",\"LastName\":\"lname-02\",\"DOB\":\"2000-10-22T00:00:00\",\"Balance\":2000.00,\"CustomField\":\"\"}", dataRow1.Json);

            Assert.AreEqual(8, dataRow1.Fields.Count);
            AssertValidField(0, "BALANCE", "BALANCE", dataRow1, dataRow1.Fields[0]);
            AssertValidField(1, "1002", 1002, dataRow1, dataRow1.Fields[1]);
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
            Assert.AreEqual(0, dataRow2.Warnings.Count);
            Assert.AreEqual("{\"RecordType\":\"BALANCE\",\"ConsumerID\":1003,\"SSN\":\"111-22-1003\",\"FirstName\":\"fname-03\",\"LastName\":\"lname-03\",\"DOB\":\"2000-10-23T00:00:00\",\"Balance\":3000.00,\"CustomField\":\"\"}", dataRow2.Json);

            Assert.AreEqual(8, dataRow2.Fields.Count);
            AssertValidField(0, "BALANCE", "BALANCE", dataRow2, dataRow2.Fields[0]);
            AssertValidField(1, "1003", 1003, dataRow2, dataRow2.Fields[1]);
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
            var fileDataSourceValidFile = CreateFileDataSource("balance-with-header-and-trailer.10.csv");
            var target = new ParsedDataProcessor10(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.AllRows);

            Assert.IsNotNull(actual.Trailer);
            Assert.AreEqual(4, actual.Trailer.Index);
            Assert.AreEqual(ValidationResultType.Valid, actual.Trailer.ValidationResult);
            Assert.AreEqual("TRAILER,6000.00,3", actual.Trailer.Raw);
            Assert.AreEqual("TRAILER,6000.00,3", string.Join(',', actual.Trailer.RawFields));
            Assert.AreEqual(0, actual.Trailer.Errors.Count);
            Assert.AreEqual(0, actual.Trailer.Warnings.Count);
            Assert.AreEqual("{\"RecordType\":\"TRAILER\",\"BalanceTotal\":6000.00,\"RecordCount\":3}", actual.Trailer.Json);

            Assert.AreEqual(3, actual.Trailer.Fields.Count);
            AssertValidField(0, "TRAILER", "TRAILER", actual.Trailer, actual.Trailer.Fields[0]);
            AssertValidField(1, "6000.00", 6000m, actual.Trailer, actual.Trailer.Fields[1]);
            AssertValidField(2, "3", 3, actual.Trailer, actual.Trailer.Fields[2]);
        }

        [TestMethod]
        public void Process_Given_an_invalid_header_Should_indicate_error()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-header.10.csv");
            var target = new ParsedDataProcessor10(fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.Errors);
            TestContext.PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(1, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(1, actual.InvalidRows.Count);

            Assert.AreEqual("Header row is invalid", actual.Errors[0]);

            Assert.AreSame(actual.Header, actual.InvalidRows[0]);
            var invalidRow = actual.Header;
            Assert.AreEqual(ValidationResultType.Error, invalidRow.ValidationResult);
            Assert.AreEqual(1, invalidRow.Errors.Count);
            Assert.AreEqual(0, invalidRow.Warnings.Count);
            Assert.AreEqual("Invalid Record Type (Header Row) 'H'", invalidRow.Errors[0]);
        }

        [TestMethod]
        public void Process_Given_a_header_with_warning_Should_indicate_warning()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-header.10.csv");

            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer-with-warnings.definition.10.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile_10>(path);
            var fileProcessorDefinitionWithWarnings = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            var target = new ParsedDataProcessor10(fileDataSource, fileProcessorDefinitionWithWarnings);

            var actual = target.Process();
            TestContext.PrintJson(actual);

            Assert.AreEqual(ValidationResultType.Warning, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);

            var rowWithWarning = actual.Header;
            Assert.AreEqual(ValidationResultType.Warning, rowWithWarning.ValidationResult);
            Assert.AreEqual(0, rowWithWarning.Errors.Count);
            Assert.AreEqual(1, rowWithWarning.Warnings.Count);
            Assert.AreEqual("Invalid Record Type (Header Row) 'H'", rowWithWarning.Warnings[0]);
        }

        [TestMethod]
        public void Process_Given_an_invalid_data_row_Should_indicate_error()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-date-in-a-data-row.10.csv");
            var target = new ParsedDataProcessor10(fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.Errors);
            TestContext.PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(1, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(1, actual.InvalidRows.Count);

            Assert.AreEqual("There is 1 invalid data row", actual.Errors[0]);

            Assert.AreSame(actual.AllRows[2], actual.InvalidRows[0]);
            
            var invalidRow = actual.AllRows[2];
            Assert.AreEqual(ValidationResultType.Error, invalidRow.ValidationResult);
            Assert.AreEqual(1, invalidRow.Errors.Count);
            Assert.AreEqual(0, invalidRow.Warnings.Count);
            Assert.AreEqual("Invalid DOB '1022200a'", invalidRow.Errors[0]);
        }

        [TestMethod]
        public void Process_Given_an_data_row_with_warning_Should_indicate_warning()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-date-in-a-data-row.10.csv");

            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer-with-warnings.definition.10.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile_10>(path);
            var fileProcessorDefinitionWithWarnings = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            var target = new ParsedDataProcessor10(fileDataSource, fileProcessorDefinitionWithWarnings);

            var actual = target.Process();
            TestContext.PrintJson(actual);
            

            Assert.AreEqual(ValidationResultType.Warning, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);

            var rowWithWarning = actual.AllRows[2];
            Assert.AreEqual(ValidationResultType.Warning, rowWithWarning.ValidationResult);
            Assert.AreEqual(0, rowWithWarning.Errors.Count);
            Assert.AreEqual(1, rowWithWarning.Warnings.Count);
            Assert.AreEqual("Invalid DOB '1022200a'", rowWithWarning.Warnings[0]);
        }

        [TestMethod]
        public void Process_Given_an_invalid_trailer_Should_indicate_error()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-trailer.10.csv");
            var target = new ParsedDataProcessor10(fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.Errors);
            TestContext.PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(1, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(1, actual.InvalidRows.Count);

            Assert.AreEqual("Trailer row is invalid", actual.Errors[0]);

            Assert.AreSame(actual.Trailer, actual.InvalidRows[0]);
            var invalidRow = actual.Trailer;
            Assert.AreEqual(ValidationResultType.Error, invalidRow.ValidationResult);
            Assert.AreEqual(1, invalidRow.Errors.Count);
            Assert.AreEqual(0, invalidRow.Warnings.Count);
            Assert.AreEqual("Invalid Balance Total '6000.oo'", invalidRow.Errors[0]);
        }

        [TestMethod]
        public void Process_Given_a_trailer_with_warning_Should_indicate_warning()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-trailer.10.csv");

            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer-with-warnings.definition.10.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile_10>(path);
            var fileProcessorDefinitionWithWarnings = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            var target = new ParsedDataProcessor10(fileDataSource, fileProcessorDefinitionWithWarnings);

            var actual = target.Process();
            TestContext.PrintJson(actual);

            Assert.AreEqual(ValidationResultType.Warning, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);

            var rowWithWarning = actual.Trailer;
            Assert.AreEqual(ValidationResultType.Warning, rowWithWarning.ValidationResult);
            Assert.AreEqual(0, rowWithWarning.Errors.Count);
            Assert.AreEqual(1, rowWithWarning.Warnings.Count);
            Assert.AreEqual("Invalid Balance Total '6000.oo'", rowWithWarning.Warnings[0]);
        }

        [TestMethod]
        public void Process_Given_multiple_rows_with_errors_Should_indicate_the_errors()
        {
            var fileDataSource = CreateFileDataSource("balance-with-multiple-errors.10.csv");
            var target = new ParsedDataProcessor10(fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.Errors);
            TestContext.PrintJson(actual.AllRows);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(2, actual.Errors.Count);
            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(3, actual.InvalidRows.Count);

            Assert.AreEqual("Header row is invalid", actual.Errors[0]);
            Assert.AreEqual("There are 2 invalid data rows", actual.Errors[1]);

            Assert.AreSame(actual.Header, actual.InvalidRows[0]);
            var invalidRow = actual.Header;
            Assert.AreEqual(ValidationResultType.Error, invalidRow.ValidationResult);
            Assert.AreEqual(1, invalidRow.Errors.Count);
            Assert.AreEqual(0, invalidRow.Warnings.Count);
            Assert.AreEqual("Invalid Record Type (Header Row) 'H'", invalidRow.Errors[0]);

            Assert.AreSame(actual.AllRows[2], actual.InvalidRows[1]);
            invalidRow = actual.AllRows[2];
            Assert.AreEqual(ValidationResultType.Error, invalidRow.ValidationResult);
            Assert.AreEqual(1, invalidRow.Errors.Count);
            Assert.AreEqual(0, invalidRow.Warnings.Count);
            Assert.AreEqual("Invalid DOB '1022200a'", invalidRow.Errors[0]);
        }

        public void AssertValidField(int expectedIndex, string expectedRaw, object expectedValue, Row expectedRow, Field actualField)
        {
            Assert.AreEqual(expectedIndex, actualField.Index);
            Assert.AreEqual(expectedRaw, actualField.Raw);
            Assert.AreEqual(expectedValue, actualField.Value);
            Assert.AreEqual(ValidationResultType.Valid, actualField.ValidationResult);
            Assert.AreSame(expectedRow, actualField.Row);
        }

        private IDataSource CreateFileDataSource(string filename)
        {
            return TestHelpers.CreateFileDataSource(filename, _inputDefinitionFile.HasFieldsEnclosedInQuotes, _inputDefinitionFile.Delimiter);
        }
    }
}
