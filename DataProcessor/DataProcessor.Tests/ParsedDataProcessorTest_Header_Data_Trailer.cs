using DataProcessor.DataSource.File;
using DataProcessor.Decoders;
using DataProcessor.Models;
using DataProcessor.ProcessorDefinition.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessorTest_Header_Data_Trailer
    {
        private FileProcessorDefinition10 _fileProcessorDefinition;
        private TextDecoder _textDecoder;
        private FileDataSource _fileDataSource;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            _fileDataSource = TestHelpers.CreateFileDataSource("test-file-header-data-trailer.csv", false);

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
                    FieldProcessorDefinitions = new FieldProcessorDefinition[]
                    {
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-TA" },
                        new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-TB" }
                    }
                }
            };
        }

        [TestMethod]
        public void Process_Given_a_file_with_header_data_and_trailer_Should_decode_and_parse_fields()
        {
            var target = new ParsedDataProcessor(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.AllRows);

            Assert.AreEqual(4, actual.AllRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);
            Assert.AreEqual(0, actual.Errors.Count);

            var row0 = actual.AllRows[0];
            Assert.IsNull(row0.Json);
            Assert.AreEqual(2, row0.Fields.Count);
            Assert.AreEqual("field-1a", row0.Fields[0].Value);
            Assert.AreEqual("field-1b", row0.Fields[1].Value);

            var row1 = actual.AllRows[1];
            Assert.IsNull(row1.Json);
            Assert.AreEqual(3, row1.Fields.Count);
            Assert.AreEqual("field-2a", row1.Fields[0].Value);
            Assert.AreEqual("field-2b", row1.Fields[1].Value);
            Assert.AreEqual("field-2c", row1.Fields[2].Value);

            var row2 = actual.AllRows[2];
            Assert.IsNull(row2.Json);
            Assert.AreEqual(3, row2.Fields.Count);
            Assert.AreEqual("field-3a", row2.Fields[0].Value);
            Assert.AreEqual("field-3b", row2.Fields[1].Value);
            Assert.AreEqual("field-3c", row2.Fields[2].Value);

            var row3 = actual.AllRows[3];
            Assert.IsNull(row3.Json);
            Assert.AreEqual(2, row3.Fields.Count);
            Assert.AreEqual("field-4a", row3.Fields[0].Value);
            Assert.AreEqual("field-4b", row3.Fields[1].Value);

            Assert.AreSame(row0, actual.Header);
            Assert.AreSame(row3, actual.Trailer);
        }

        [TestMethod]
        public void Process_Given_a_file_with_header_data_and_trailer_Should_create_the_json_for_the_rows()
        {
            _fileProcessorDefinition.CreateRowJsonEnabled = true;
            var target = new ParsedDataProcessor(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.AllRows);
            TestContext.PrintRowJsons(actual.AllRows);

            Assert.AreEqual(4, actual.AllRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);
            Assert.AreEqual(0, actual.Errors.Count);

            var row0 = actual.AllRows[0];
            Assert.AreEqual("{\"Field-HA\":\"field-1a\",\"Field-HB\":\"field-1b\"}", row0.Json);

            var row1 = actual.AllRows[1];
            Assert.AreEqual("{\"Field-DA\":\"field-2a\",\"Field-DB\":\"field-2b\",\"Field-DC\":\"field-2c\"}", row1.Json);

            var row2 = actual.AllRows[2];
            Assert.AreEqual("{\"Field-DA\":\"field-3a\",\"Field-DB\":\"field-3b\",\"Field-DC\":\"field-3c\"}", row2.Json);

            var row3 = actual.AllRows[3];
            Assert.AreEqual("{\"Field-TA\":\"field-4a\",\"Field-TB\":\"field-4b\"}", row3.Json);
        }
    }
}
