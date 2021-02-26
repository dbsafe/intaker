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
    public class ParsedDataProcessor20Test_Integration_Decoders
    {
        private ProcessorDefinition.Models.FileProcessorDefinition20 _fileProcessorDefinition;
        private InputDefinitionFile20 _inputDefinitionFile;
        private readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DataProcessorGlobal.IsDebugEnabled = true;
            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer.definition.20.xml");
            _inputDefinitionFile = FileLoader.Load<InputDefinitionFile20>(path);
            _fileProcessorDefinition = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(_inputDefinitionFile);
        }

        [TestMethod]
        public void Process_Given_a_valid_file_There_should_no_be_errors()
        {
            var fileDataSourceValidFile = CreateFileDataSource("balance-with-header-and-trailer.20.csv");
            var target = new ParsedDataProcessor20(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();

            Assert.AreEqual(ValidationResultType.Valid, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);
        }

        [TestMethod]
        public void Process_Given_a_valid_file_Header_should_be_decoded()
        {
            var fileDataSourceValidFile = CreateFileDataSource("balance-with-header-and-trailer.20.csv");
            var target = new ParsedDataProcessor20(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintRowJsons(actual.DataRows);

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
            var fileDataSourceValidFile = CreateFileDataSource("balance-with-header-and-trailer.20.csv");
            var target = new ParsedDataProcessor20(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();

            var dataRow0_Balance = actual.DataRows[0].Row;
            Assert.AreEqual(1, dataRow0_Balance.Index);
            Assert.AreEqual(ValidationResultType.Valid, dataRow0_Balance.ValidationResult);
            Assert.AreEqual("BALANCE,1001,111-22-1001,fname-01,lname-01,10212000,1000.00,AA", dataRow0_Balance.Raw);
            Assert.AreEqual("BALANCE,1001,111-22-1001,fname-01,lname-01,10212000,1000.00,AA", string.Join(',', dataRow0_Balance.RawFields));
            Assert.AreEqual(0, dataRow0_Balance.Errors.Count);
            Assert.AreEqual(0, dataRow0_Balance.Warnings.Count);
            Assert.AreEqual("{\"RecordType\":\"BALANCE\",\"ConsumerID\":1001,\"SSN\":\"111-22-1001\",\"FirstName\":\"fname-01\",\"LastName\":\"lname-01\",\"DOB\":\"2000-10-21T00:00:00\",\"Balance\":1000.00,\"CustomField\":\"AA\"}", dataRow0_Balance.Json);

            Assert.AreEqual(8, dataRow0_Balance.Fields.Count);
            AssertValidField(0, "BALANCE", "BALANCE", dataRow0_Balance, dataRow0_Balance.Fields[0]);
            AssertValidField(1, "1001", 1001, dataRow0_Balance, dataRow0_Balance.Fields[1]);
            AssertValidField(2, "111-22-1001", "111-22-1001", dataRow0_Balance, dataRow0_Balance.Fields[2]);
            AssertValidField(3, "fname-01", "fname-01", dataRow0_Balance, dataRow0_Balance.Fields[3]);
            AssertValidField(4, "lname-01", "lname-01", dataRow0_Balance, dataRow0_Balance.Fields[4]);
            AssertValidField(5, "10212000", new DateTime(2000, 10, 21), dataRow0_Balance, dataRow0_Balance.Fields[5]);
            AssertValidField(6, "1000.00", 1000m, dataRow0_Balance, dataRow0_Balance.Fields[6]);
            AssertValidField(7, "AA", "AA", dataRow0_Balance, dataRow0_Balance.Fields[7]);

            var dataRow1_Balance = actual.DataRows[1].Row;
            Assert.AreEqual(2, dataRow1_Balance.Index);
            Assert.AreEqual(ValidationResultType.Valid, dataRow1_Balance.ValidationResult);
            Assert.AreEqual("BALANCE,1002,111-22-1002,fname-02,lname-02,10222000,2000.00,", dataRow1_Balance.Raw);
            Assert.AreEqual("BALANCE,1002,111-22-1002,fname-02,lname-02,10222000,2000.00,", string.Join(',', dataRow1_Balance.RawFields));
            Assert.AreEqual(0, dataRow1_Balance.Errors.Count);
            Assert.AreEqual(0, dataRow1_Balance.Warnings.Count);
            Assert.AreEqual("{\"RecordType\":\"BALANCE\",\"ConsumerID\":1002,\"SSN\":\"111-22-1002\",\"FirstName\":\"fname-02\",\"LastName\":\"lname-02\",\"DOB\":\"2000-10-22T00:00:00\",\"Balance\":2000.00,\"CustomField\":\"\"}", dataRow1_Balance.Json);

            Assert.AreEqual(8, dataRow1_Balance.Fields.Count);
            AssertValidField(0, "BALANCE", "BALANCE", dataRow1_Balance, dataRow1_Balance.Fields[0]);
            AssertValidField(1, "1002", 1002, dataRow1_Balance, dataRow1_Balance.Fields[1]);
            AssertValidField(2, "111-22-1002", "111-22-1002", dataRow1_Balance, dataRow1_Balance.Fields[2]);
            AssertValidField(3, "fname-02", "fname-02", dataRow1_Balance, dataRow1_Balance.Fields[3]);
            AssertValidField(4, "lname-02", "lname-02", dataRow1_Balance, dataRow1_Balance.Fields[4]);
            AssertValidField(5, "10222000", new DateTime(2000, 10, 22), dataRow1_Balance, dataRow1_Balance.Fields[5]);
            AssertValidField(6, "2000.00", 2000m, dataRow1_Balance, dataRow1_Balance.Fields[6]);
            AssertValidField(7, "", "", dataRow1_Balance, dataRow1_Balance.Fields[7]);

            var dataRow2_Change = actual.DataRows[2].Row;
            Assert.AreEqual(3, dataRow2_Change.Index);
            Assert.AreEqual(ValidationResultType.Valid, dataRow2_Change.ValidationResult);
            Assert.AreEqual("CHANGE,1002,01022020,1002-addr-1,1002-addr-2", dataRow2_Change.Raw);
            Assert.AreEqual("CHANGE,1002,01022020,1002-addr-1,1002-addr-2", string.Join(',', dataRow2_Change.RawFields));
            Assert.AreEqual(0, dataRow2_Change.Errors.Count);
            Assert.AreEqual(0, dataRow2_Change.Warnings.Count);
            Assert.AreEqual("{\"RecordType\":\"CHANGE\",\"ConsumerID\":1002,\"Date\":\"2020-01-02T00:00:00\",\"AddressLine1\":\"1002-addr-1\",\"AddressLine2\":\"1002-addr-2\"}", dataRow2_Change.Json);

            Assert.AreEqual(5, dataRow2_Change.Fields.Count);
            AssertValidField(0, "CHANGE", "CHANGE", dataRow2_Change, dataRow2_Change.Fields[0]);
            AssertValidField(1, "1002", 1002, dataRow2_Change, dataRow2_Change.Fields[1]);
            AssertValidField(2, "01022020", new DateTime(2020, 1, 2), dataRow2_Change, dataRow2_Change.Fields[2]);
            AssertValidField(3, "1002-addr-1", "1002-addr-1", dataRow2_Change, dataRow2_Change.Fields[3]);
            AssertValidField(4, "1002-addr-2", "1002-addr-2", dataRow2_Change, dataRow2_Change.Fields[4]);

            var dataRow3_Balance = actual.DataRows[3].Row;
            Assert.AreEqual(4, dataRow3_Balance.Index);
            Assert.AreEqual(ValidationResultType.Valid, dataRow3_Balance.ValidationResult);
            Assert.AreEqual("BALANCE,1003,111-22-1003,fname-03,lname-03,10232000,3000.00,", dataRow3_Balance.Raw);
            Assert.AreEqual("BALANCE,1003,111-22-1003,fname-03,lname-03,10232000,3000.00,", string.Join(',', dataRow3_Balance.RawFields));
            Assert.AreEqual(0, dataRow3_Balance.Errors.Count);
            Assert.AreEqual(0, dataRow3_Balance.Warnings.Count);
            Assert.AreEqual("{\"RecordType\":\"BALANCE\",\"ConsumerID\":1003,\"SSN\":\"111-22-1003\",\"FirstName\":\"fname-03\",\"LastName\":\"lname-03\",\"DOB\":\"2000-10-23T00:00:00\",\"Balance\":3000.00,\"CustomField\":\"\"}", dataRow3_Balance.Json);

            Assert.AreEqual(8, dataRow3_Balance.Fields.Count);
            AssertValidField(0, "BALANCE", "BALANCE", dataRow3_Balance, dataRow3_Balance.Fields[0]);
            AssertValidField(1, "1003", 1003, dataRow3_Balance, dataRow3_Balance.Fields[1]);
            AssertValidField(2, "111-22-1003", "111-22-1003", dataRow3_Balance, dataRow3_Balance.Fields[2]);
            AssertValidField(3, "fname-03", "fname-03", dataRow3_Balance, dataRow3_Balance.Fields[3]);
            AssertValidField(4, "lname-03", "lname-03", dataRow3_Balance, dataRow3_Balance.Fields[4]);
            AssertValidField(5, "10232000", new DateTime(2000, 10, 23), dataRow3_Balance, dataRow3_Balance.Fields[5]);
            AssertValidField(6, "3000.00", 3000m, dataRow3_Balance, dataRow3_Balance.Fields[6]);
            AssertValidField(7, "", "", dataRow3_Balance, dataRow3_Balance.Fields[7]);

            var dataRow4_Change = actual.DataRows[4].Row;
            Assert.AreEqual(5, dataRow4_Change.Index);
            Assert.AreEqual(ValidationResultType.Valid, dataRow4_Change.ValidationResult);
            Assert.AreEqual("CHANGE,1003,01032020,1003-addr-1,1003-addr-2", dataRow4_Change.Raw);
            Assert.AreEqual("CHANGE,1003,01032020,1003-addr-1,1003-addr-2", string.Join(',', dataRow4_Change.RawFields));
            Assert.AreEqual(0, dataRow4_Change.Errors.Count);
            Assert.AreEqual(0, dataRow4_Change.Warnings.Count);
            Assert.AreEqual("{\"RecordType\":\"CHANGE\",\"ConsumerID\":1003,\"Date\":\"2020-01-03T00:00:00\",\"AddressLine1\":\"1003-addr-1\",\"AddressLine2\":\"1003-addr-2\"}", dataRow4_Change.Json);

            Assert.AreEqual(5, dataRow4_Change.Fields.Count);
            AssertValidField(0, "CHANGE", "CHANGE", dataRow4_Change, dataRow4_Change.Fields[0]);
            AssertValidField(1, "1003", 1003, dataRow4_Change, dataRow4_Change.Fields[1]);
            AssertValidField(2, "01032020", new DateTime(2020, 1, 3), dataRow4_Change, dataRow4_Change.Fields[2]);
            AssertValidField(3, "1003-addr-1", "1003-addr-1", dataRow4_Change, dataRow4_Change.Fields[3]);
            AssertValidField(4, "1003-addr-2", "1003-addr-2", dataRow4_Change, dataRow4_Change.Fields[4]);
        }

        [TestMethod]
        public void Process_Given_a_valid_file_Trailer_should_be_decoded()
        {
            var fileDataSourceValidFile = CreateFileDataSource("balance-with-header-and-trailer.20.csv");
            var target = new ParsedDataProcessor20(fileDataSourceValidFile, _fileProcessorDefinition);

            var actual = target.Process();

            Assert.IsNotNull(actual.Trailer);
            Assert.AreEqual(6, actual.Trailer.Index);
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
            var fileDataSource = CreateFileDataSource("balance-with-invalid-header.20.csv");
            var target = new ParsedDataProcessor20(fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.Errors);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(1, actual.Errors.Count);
            Assert.AreEqual(5, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);

            Assert.AreEqual("Header row is invalid", actual.Errors[0]);

            Assert.AreEqual(ValidationResultType.Error, actual.Header.ValidationResult);
            Assert.AreEqual(1, actual.Header.Errors.Count);
            Assert.AreEqual(0, actual.Header.Warnings.Count);
            Assert.AreEqual("Invalid Record Type (Header Row) 'H'", actual.Header.Errors[0]);
        }

        [TestMethod]
        public void Process_Given_a_header_with_warning_Should_indicate_warning()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-header.20.csv");

            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer-with-warnings.definition.20.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile20>(path);
            var fileProcessorDefinitionWithWarnings = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            var target = new ParsedDataProcessor20(fileDataSource, fileProcessorDefinitionWithWarnings);

            var actual = target.Process();
            TestContext.PrintJson(actual);

            Assert.AreEqual(ValidationResultType.Warning, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);

            Assert.AreEqual(ValidationResultType.Warning, actual.Header.ValidationResult);
            Assert.AreEqual(0, actual.Header.Errors.Count);
            Assert.AreEqual(1, actual.Header.Warnings.Count);
            Assert.AreEqual("Invalid Record Type (Header Row) 'H'", actual.Header.Warnings[0]);
        }

        [TestMethod]
        public void Process_Given_an_invalid_data_row_Should_indicate_error()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-date-in-a-data-row.20.csv");
            var target = new ParsedDataProcessor20(fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.Errors);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(1, actual.Errors.Count);
            Assert.AreEqual(5, actual.DataRows.Count);
            Assert.AreEqual(1, actual.InvalidDataRows.Count);

            Assert.AreEqual("There is 1 invalid data row", actual.Errors[0]);

            Assert.AreSame(actual.DataRows[1], actual.InvalidDataRows[0]);

            var invalidRow = actual.DataRows[1].Row;
            Assert.AreEqual(ValidationResultType.Error, invalidRow.ValidationResult);
            Assert.AreEqual(1, invalidRow.Errors.Count);
            Assert.AreEqual(0, invalidRow.Warnings.Count);
            Assert.AreEqual("Invalid DOB '1022200a'", invalidRow.Errors[0]);
        }

        [TestMethod]
        public void Process_Given_an_data_row_with_warning_Should_indicate_warning()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-date-in-a-data-row.20.csv");

            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer-with-warnings.definition.20.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile20>(path);
            var fileProcessorDefinitionWithWarnings = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            var target = new ParsedDataProcessor20(fileDataSource, fileProcessorDefinitionWithWarnings);

            var actual = target.Process();
            TestContext.PrintJson(actual);


            Assert.AreEqual(ValidationResultType.Warning, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);

            var rowWithWarning = actual.DataRows[1].Row;
            Assert.AreEqual(ValidationResultType.Warning, rowWithWarning.ValidationResult);
            Assert.AreEqual(0, rowWithWarning.Errors.Count);
            Assert.AreEqual(1, rowWithWarning.Warnings.Count);
            Assert.AreEqual("Invalid DOB '1022200a'", rowWithWarning.Warnings[0]);
        }

        [TestMethod]
        public void Process_Given_an_invalid_trailer_Should_indicate_error()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-trailer.20.csv");
            var target = new ParsedDataProcessor20(fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.Errors);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(1, actual.Errors.Count);
            Assert.AreEqual(5, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);

            Assert.AreEqual("Trailer row is invalid", actual.Errors[0]);

            Assert.AreEqual(ValidationResultType.Error, actual.Trailer.ValidationResult);
            Assert.AreEqual(1, actual.Trailer.Errors.Count);
            Assert.AreEqual(0, actual.Trailer.Warnings.Count);
            Assert.AreEqual("Invalid Balance Total '6000.oo'", actual.Trailer.Errors[0]);
        }

        [TestMethod]
        public void Process_Given_a_trailer_with_warning_Should_indicate_warning()
        {
            var fileDataSource = CreateFileDataSource("balance-with-invalid-trailer.20.csv");

            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer-with-warnings.definition.20.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile20>(path);
            var fileProcessorDefinitionWithWarnings = ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            var target = new ParsedDataProcessor20(fileDataSource, fileProcessorDefinitionWithWarnings);

            var actual = target.Process();
            TestContext.PrintJson(actual);

            Assert.AreEqual(ValidationResultType.Warning, actual.ValidationResult);
            Assert.AreEqual(0, actual.Errors.Count);
            Assert.AreEqual(5, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);

            Assert.AreEqual(ValidationResultType.Warning, actual.Trailer.ValidationResult);
            Assert.AreEqual(0, actual.Trailer.Errors.Count);
            Assert.AreEqual(1, actual.Trailer.Warnings.Count);
            Assert.AreEqual("Invalid Balance Total '6000.oo'", actual.Trailer.Warnings[0]);
        }

        [TestMethod]
        public void Process_Given_multiple_rows_with_errors_Should_indicate_the_errors()
        {
            var fileDataSource = CreateFileDataSource("balance-with-multiple-errors.20.csv");
            var target = new ParsedDataProcessor20(fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.Errors);

            Assert.AreEqual(ValidationResultType.Error, actual.ValidationResult);
            Assert.AreEqual(2, actual.Errors.Count);
            Assert.AreEqual(5, actual.DataRows.Count);
            Assert.AreEqual(2, actual.InvalidDataRows.Count);

            Assert.AreEqual("Header row is invalid", actual.Errors[0]);
            Assert.AreEqual("There are 2 invalid data rows", actual.Errors[1]);

            Assert.AreEqual(ValidationResultType.Error, actual.Header.ValidationResult);
            Assert.AreEqual(1, actual.Header.Errors.Count);
            Assert.AreEqual(0, actual.Header.Warnings.Count);
            Assert.AreEqual("Invalid Record Type (Header Row) 'H'", actual.Header.Errors[0]);

            Assert.AreSame(actual.DataRows[1], actual.InvalidDataRows[0]);
            var dataRow1 = actual.DataRows[1].Row;
            Assert.AreEqual(ValidationResultType.Error, dataRow1.ValidationResult);
            Assert.AreEqual(1, dataRow1.Errors.Count);
            Assert.AreEqual(0, dataRow1.Warnings.Count);
            Assert.AreEqual("Invalid DOB '1022200a'", dataRow1.Errors[0]);

            Assert.AreSame(actual.DataRows[3], actual.InvalidDataRows[1]);
            var dataRow3 = actual.DataRows[3].Row;
            Assert.AreEqual(ValidationResultType.Error, dataRow3.ValidationResult);
            Assert.AreEqual(1, dataRow3.Errors.Count);
            Assert.AreEqual(0, dataRow3.Warnings.Count);
            Assert.AreEqual("Invalid DOB '1023200b'", dataRow3.Errors[0]);
        }

        public void AssertValidField(int expectedIndex, string expectedRaw, object expectedValue, Row expectedRow, Field actualField)
        {
            Assert.AreEqual(expectedIndex, actualField.Index);
            Assert.AreEqual(expectedRaw, actualField.Raw);
            Assert.AreEqual(expectedValue, actualField.Value);
            Assert.AreEqual(ValidationResultType.Valid, actualField.ValidationResult);
            Assert.AreSame(expectedRow, actualField.Row);
        }

        private IDataSource<ParserContext20> CreateFileDataSource(string filename)
        {
            return TestHelpers.CreateFileDataSource<ParserContext20>(
                filename, 
                _inputDefinitionFile.HasFieldsEnclosedInQuotes, 
                _inputDefinitionFile.Delimiter,
                _inputDefinitionFile.CommentedOutIndicator);
        }
    }
}
