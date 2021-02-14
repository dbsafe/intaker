using DataProcessor.DataSource.File;
using DataProcessor.Decoders;
using DataProcessor.Models;
using DataProcessor.ProcessorDefinition.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessor10Test_Data_Tailer
    {
        private FileProcessorDefinition10 _fileProcessorDefinition;
        private TextDecoder _textDecoder;
        private FileDataSource<ParserContext10> _fileDataSource;

        [TestInitialize]
        public void Initialize()
        {
            _fileDataSource = TestHelpers.CreateFileDataSource<ParserContext10>("test-file-data-trailer.10.csv", false);

            _textDecoder = new TextDecoder { Pattern = @"*.", FailValidationResult = ValidationResultType.Critical };
            _fileProcessorDefinition = new FileProcessorDefinition10
            {
                HeaderRowProcessorDefinition = new RowProcessorDefinition
                {
                    FieldProcessorDefinitions = new FieldProcessorDefinition[] { },
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
                    FieldProcessorDefinitions = new FieldProcessorDefinition[]
                    {
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-TA" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-TB" }
                    }
                }
            };
        }

        [TestMethod]
        public void Process_Given_a_file_with_data_and_trailer_Should_decode_and_parse_fields()
        {
            var target = new ParsedDataProcessor10(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();

            Assert.IsNull(actual.Header);
            Assert.AreEqual(2, actual.DataRows.Count);
            Assert.IsNotNull(actual.Trailer);
            Assert.AreEqual(0, actual.InvalidDataRows.Count);
            Assert.AreEqual(0, actual.Errors.Count);


            var dataRow0 = actual.DataRows[0];
            Assert.AreEqual(3, dataRow0.Fields.Count);
            Assert.AreEqual("field-1a", dataRow0.Fields[0].Value);
            Assert.AreEqual("field-1b", dataRow0.Fields[1].Value);
            Assert.AreEqual("field-1c", dataRow0.Fields[2].Value);

            var dataRow1 = actual.DataRows[1];
            Assert.AreEqual(3, dataRow1.Fields.Count);
            Assert.AreEqual("field-2a", dataRow1.Fields[0].Value);
            Assert.AreEqual("field-2b", dataRow1.Fields[1].Value);
            Assert.AreEqual("field-2c", dataRow1.Fields[2].Value);

            Assert.AreEqual(2, actual.Trailer.Fields.Count);
            Assert.AreEqual("field-3a", actual.Trailer.Fields[0].Value);
            Assert.AreEqual("field-3b", actual.Trailer.Fields[1].Value);
        }
    }
}
