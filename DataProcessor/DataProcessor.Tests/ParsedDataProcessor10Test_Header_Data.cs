using DataProcessor.DataSource.File;
using DataProcessor.Decoders;
using DataProcessor.Models;
using DataProcessor.ProcessorDefinition.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessor10Test_Header_Data
    {
        private FileProcessorDefinition10 _fileProcessorDefinition;
        private TextDecoder _textDecoder;
        private FileDataSource<ParserContext10> _fileDataSource;

        [TestInitialize]
        public void Initialize()
        {
            _fileDataSource = TestHelpers.CreateFileDataSource<ParserContext10>("test-file-header-data.10.csv", false);

            _textDecoder = new TextDecoder { Pattern = @"*.", FailValidationResult = ValidationResultType.Critical };
            _fileProcessorDefinition = new FileProcessorDefinition10
            {
                HeaderRowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[]
                    {
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-HA" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-HB" }
                    },
                },
                DataRowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[]
                    {
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-DA" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-DB" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-DC" }
                    }
                },
                TrailerRowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[] { }
                }
            };
        }

        [TestMethod]
        public void Process_Given_a_file_with_header_Should_decode_and_parse_fields()
        {
            var target = new ParsedDataProcessor10(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();

            Assert.AreEqual(2, actual.DataRows.Count);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);
            Assert.AreEqual(0, actual.Errors.Count);

            Assert.AreEqual(2, actual.Header.Fields.Count);
            Assert.AreEqual("field-1a", actual.Header.Fields[0].Value);
            Assert.AreEqual("field-1b", actual.Header.Fields[1].Value);

            var dataRow0 = actual.DataRows[0];
            Assert.AreEqual(3, dataRow0.Fields.Count);
            Assert.AreEqual("field-2a", dataRow0.Fields[0].Value);
            Assert.AreEqual("field-2b", dataRow0.Fields[1].Value);
            Assert.AreEqual("field-2c", dataRow0.Fields[2].Value);

            var dataRow1 = actual.DataRows[1];
            Assert.AreEqual(3, dataRow1.Fields.Count);
            Assert.AreEqual("field-3a", dataRow1.Fields[0].Value);
            Assert.AreEqual("field-3b", dataRow1.Fields[1].Value);
            Assert.AreEqual("field-3c", dataRow1.Fields[2].Value);
            
            Assert.IsNull(actual.Trailer);
        }
    }
}
