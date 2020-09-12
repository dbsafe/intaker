using DataProcessor.DataSource.File;
using DataProcessor.Decoders;
using DataProcessor.Domain.Models;
using DataProcessor.ProcessorDefinition.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessorTest_Data
    {
        private FileProcessorDefinition _fileProcessorDefinition;
        private TextDecoder _textDecoder;
        private FileDataSource _fileDataSource;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            _fileDataSource = TestHelpers.CreateFileDataSource("test-file-data.csv", false);

            _textDecoder = new TextDecoder { Pattern = @"*.", FailValidationResult = ValidationResultType.Critical };
            _fileProcessorDefinition = new FileProcessorDefinition
            {
                HeaderRowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[] { },
                },
                DataRowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[]
                    {
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "FieldA", Description = "Field A" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "FieldB", Description = "Field B" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "FieldC", Description = "Field C" }
                    }
                },
                TrailerRowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[] { }
                }
            };
        }

        [TestMethod]
        public void Process_Given_a_file_without_header_and_trailer_Should_decode_and_parse_fields()
        {
            var target = new ParsedDataProcessor(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();

            Assert.AreEqual(3, actual.AllRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);
            Assert.AreEqual(0, actual.Errors.Count);

            var row0 = actual.AllRows[0];
            Assert.AreEqual(3, row0.Fields.Count);
            Assert.AreEqual("field-1a", row0.Fields[0].Value);
            Assert.AreEqual("field-1b", row0.Fields[1].Value);
            Assert.AreEqual("field-1c", row0.Fields[2].Value);

            var row1 = actual.AllRows[1];
            Assert.AreEqual(3, row1.Fields.Count);
            Assert.AreEqual("field-2a", row1.Fields[0].Value);
            Assert.AreEqual("field-2b", row1.Fields[1].Value);
            Assert.AreEqual("field-2c", row1.Fields[2].Value);

            var row2 = actual.AllRows[2];
            Assert.AreEqual(3, row2.Fields.Count);
            Assert.AreEqual("field-3a", row2.Fields[0].Value);
            Assert.AreEqual("field-3b", row2.Fields[1].Value);
            Assert.AreEqual("field-3c", row2.Fields[2].Value);

            Assert.IsNull(actual.Header);
            Assert.IsNull(actual.Trailer);
        }

        [TestMethod]
        public void Process_Given_that_the_number_of_fields_dont_match_The_row_should_indicate_the_error()
        {
            _fileProcessorDefinition.DataRowProcessorDefinition.FieldProcessorDefinitions = new FieldProcessorDefinition[]
            {
                new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "FieldA" },
                new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "FieldB" }
            };

            var target = new ParsedDataProcessor(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();

            Assert.AreEqual(3, actual.AllRows.Count);
            Assert.AreEqual(3, actual.InvalidRows.Count);

            var row0 = actual.AllRows[0];
            Assert.AreEqual(1, row0.Errors.Count);
            Assert.AreEqual("Data Row - The expected number of fields 2 is not equal to the actual number of fields 3", row0.Errors[0]);
        }
    }
}
