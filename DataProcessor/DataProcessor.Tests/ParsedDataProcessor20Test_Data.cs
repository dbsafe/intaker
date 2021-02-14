using DataProcessor.DataSource.File;
using DataProcessor.Decoders;
using DataProcessor.Models;
using DataProcessor.ProcessorDefinition.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessor20Test_Data
    {
        private FileProcessorDefinition20 _fileProcessorDefinition;
        private TextDecoder _textDecoder;
        private FileDataSource<ParserContext20> _fileDataSource;

        private DataRowProcessorDefinition _dataType1;
        private DataRowProcessorDefinition _dataType2;

        [TestInitialize]
        public void Initialize()
        {
            _fileDataSource = TestHelpers.CreateFileDataSource<ParserContext20>("test-file-data.20.csv", false);

            _textDecoder = new TextDecoder { Pattern = @"*.", FailValidationResult = ValidationResultType.Critical };


            _dataType1 = new DataRowProcessorDefinition
            {
                DataTypeFieldIndex = 0,
                RowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[]
                    {
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "DataType", Description = "DT1 Field A" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "key-field", Description = "DT1 Field B" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "DT1-Field-c", Description = "DT1 Field C" }
                    }
                }
            };

            _dataType2 = new DataRowProcessorDefinition
            {
                DataTypeFieldIndex = 0,
                RowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[]
                    {
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "DataType", Description = "DT2 Field A" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "key-field", Description = "DT2 Field B" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "DT2-Field-c", Description = "DT2 Field C" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "DT2-Field-d", Description = "DT2 Field D" }
                    }
                }
            };

            _fileProcessorDefinition = new FileProcessorDefinition20
            {
                DataTypeField = "FieldA",
                KeyField = "key-field",
                HeaderRowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[] { },
                },
                TrailerRowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[] { }
                },
                DataRowProcessorDefinitions = new Dictionary<string, DataRowProcessorDefinition>
                {
                    { "dt1", _dataType1 },
                    { "dt2", _dataType2 }
                }
            };
        }

        [TestMethod]
        public void Process_Given_a_file_without_header_and_trailer_Should_decode_and_parse_fields()
        {
            var target = new ParsedDataProcessor20(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();

            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);
            Assert.AreEqual(0, actual.Errors.Count);

            var dataRow0 = actual.DataRows[0].Row;
            Assert.AreEqual(3, dataRow0.Fields.Count);
            Assert.AreEqual("dt1", dataRow0.Fields[0].Value);
            Assert.AreEqual("key-value", dataRow0.Fields[1].Value);
            Assert.AreEqual("field-1c", dataRow0.Fields[2].Value);

            var dataRow1 = actual.DataRows[1].Row;
            Assert.AreEqual(3, dataRow1.Fields.Count);
            Assert.AreEqual("dt1", dataRow1.Fields[0].Value);
            Assert.AreEqual("key-value", dataRow1.Fields[1].Value);
            Assert.AreEqual("field-2c", dataRow1.Fields[2].Value);

            var dataRow2 = actual.DataRows[2].Row;
            Assert.AreEqual(4, dataRow2.Fields.Count);
            Assert.AreEqual("dt2", dataRow2.Fields[0].Value);
            Assert.AreEqual("field-3b", dataRow2.Fields[1].Value);
            Assert.AreEqual("key-value", dataRow2.Fields[2].Value);
            Assert.AreEqual("field-3d", dataRow2.Fields[3].Value);

            Assert.IsNull(actual.Header);
            Assert.IsNull(actual.Trailer);
        }

        [TestMethod]
        public void Process_Given_that_the_number_of_fields_dont_match_The_row_should_indicate_the_error()
        {
            _dataType1.RowProcessorDefinition.FieldProcessorDefinitions = new FieldProcessorDefinition[]
            {
                new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "DataType", Description = "DT1 Field A" },
                new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "key-field", Description = "DT1 Field B" }
            };

            var target = new ParsedDataProcessor20(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();

            Assert.AreEqual(3, actual.DataRows.Count);
            Assert.AreEqual(2, actual.InvalidDataRows.Count);

            var dataRow0 = actual.DataRows[0];
            Assert.AreEqual(1, dataRow0.Row.Errors.Count);
            Assert.AreEqual("Data Row 'dt1' - The expected number of fields 2 is not equal to the actual number of fields 3", dataRow0.Row.Errors[0]);
        }
    }
}
