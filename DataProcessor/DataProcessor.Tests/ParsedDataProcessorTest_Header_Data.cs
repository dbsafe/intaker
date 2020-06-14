using DataProcessor.DataSource.File;
using DataProcessor.Decoders;
using DataProcessor.Domain.Models;
using DataProcessor.ProcessorDefinition.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessorTest_Header_Data
    {
        private FileProcessorDefinition _fileProcessorDefinition;
        private TextDecoder _textDecoder;
        private FileDataSource _fileDataSource;

        [TestInitialize]
        public void Initialize()
        {
            _fileDataSource = TestHelpers.CreateFileDataSource("test-file-header-data.csv", false);

            _textDecoder = new TextDecoder { Pattern = @"*.", FailValidationResult = ValidationResultType.InvalidCritical };
            _fileProcessorDefinition = new FileProcessorDefinition
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
            var target = new ParsedDataProcessor(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();

            Assert.AreEqual(3, actual.AllRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);
            Assert.AreEqual(0, actual.Errors.Count);

            var row0 = actual.AllRows[0];
            Assert.AreEqual(2, row0.Fields.Count);
            Assert.AreEqual("field-1a", row0.Fields[0].Value);
            Assert.AreEqual("field-1b", row0.Fields[1].Value);

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

            Assert.AreSame(row0, actual.Header);
            Assert.IsNull(actual.Trailer);
        }
    }
}
