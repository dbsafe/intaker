using DataProcessor.DataSource.File;
using DataProcessor.Decoders;
using DataProcessor.Models;
using DataProcessor.ProcessorDefinition.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DataProcessor.Tests
{
    [TestClass]
    public class ParsedDataProcessor20Test_Header_Data_Trailer
    {
        private FileProcessorDefinition20 _fileProcessorDefinition;
        private TextDecoder _textDecoder;
        private FileDataSource<ParserContext20> _fileDataSource;

        private RowProcessorDefinition _header;
        private DataRowProcessorDefinition _dataType1;
        private DataRowProcessorDefinition _dataType2;
        private RowProcessorDefinition _trailer;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            _fileDataSource = TestHelpers.CreateFileDataSource<ParserContext20>("test-file-header-data-trailer.20.csv", false);

            _textDecoder = new TextDecoder { Pattern = @"*.", FailValidationResult = ValidationResultType.Critical };

            _header = new RowProcessorDefinition
            {
                FieldProcessorDefinitions = new FieldProcessorDefinition[]
                {
                    new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-HA" },
                    new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-HB" }
                }
            };

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

            _trailer = new RowProcessorDefinition
            {
                FieldProcessorDefinitions = new FieldProcessorDefinition[]
                {
                    new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-TA" },
                    new FieldProcessorDefinition { Decoder = _textDecoder, FieldName = "Field-TB" }
                }
            };

            _fileProcessorDefinition = new FileProcessorDefinition20
            {
                DataTypeField = "FieldA",
                KeyField = "key-field",
                HeaderRowProcessorDefinition = _header,
                TrailerRowProcessorDefinition = _trailer,
                DataRowProcessorDefinitions = new Dictionary<string, DataRowProcessorDefinition>
                {
                    { "dt1", _dataType1 },
                    { "dt2", _dataType2 }
                }
            };
        }

        [TestMethod]
        public void Process_Given_a_file_with_header_data_and_trailer_Should_decode_and_parse_fields()
        {
            var target = new ParsedDataProcessor20(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();

            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);
            Assert.AreEqual(0, actual.Errors.Count);

            var row0 = actual.AllRows[0];
            Assert.AreEqual(2, row0.Fields.Count);
            Assert.AreEqual("h1", row0.Fields[0].Value);
            Assert.AreEqual("h2", row0.Fields[1].Value);

            var row1 = actual.AllRows[1];
            Assert.AreEqual(3, row1.Fields.Count);
            Assert.AreEqual("dt1", row1.Fields[0].Value);
            Assert.AreEqual("key-value", row1.Fields[1].Value);
            Assert.AreEqual("field-1c", row1.Fields[2].Value);

            var row2 = actual.AllRows[2];
            Assert.AreEqual(3, row2.Fields.Count);
            Assert.AreEqual("dt1", row2.Fields[0].Value);
            Assert.AreEqual("key-value", row2.Fields[1].Value);
            Assert.AreEqual("field-2c", row2.Fields[2].Value);

            var row3 = actual.AllRows[3];
            Assert.AreEqual(4, row3.Fields.Count);
            Assert.AreEqual("dt2", row3.Fields[0].Value);
            Assert.AreEqual("field-3b", row3.Fields[1].Value);
            Assert.AreEqual("key-value", row3.Fields[2].Value);
            Assert.AreEqual("field-3d", row3.Fields[3].Value);

            var row4 = actual.AllRows[4];
            Assert.AreEqual(2, row4.Fields.Count);
            Assert.AreEqual("t1", row4.Fields[0].Value);
            Assert.AreEqual("t2", row4.Fields[1].Value);


            Assert.IsNotNull(actual.Header);
            Assert.AreEqual(row0, actual.Header);
            
            Assert.IsNotNull(actual.Trailer);
            Assert.AreEqual(row4, actual.Trailer);
        }

        [TestMethod]
        public void Process_Given_a_file_with_header_data_and_trailer_Should_create_the_json_for_the_rows()
        {
            _fileProcessorDefinition.CreateRowJsonEnabled = true;
            var target = new ParsedDataProcessor20(_fileDataSource, _fileProcessorDefinition);

            var actual = target.Process();
            TestContext.PrintJson(actual.AllRows);
            TestContext.PrintRowJsons(actual.AllRows);

            Assert.AreEqual(5, actual.AllRows.Count);
            Assert.AreEqual(0, actual.InvalidRows.Count);
            Assert.AreEqual(0, actual.Errors.Count);

            var row0 = actual.AllRows[0];
            Assert.AreEqual("{\"Field-HA\":\"h1\",\"Field-HB\":\"h2\"}", row0.Json);

            var row1 = actual.AllRows[1];
            Assert.AreEqual("{\"DataType\":\"dt1\",\"key-field\":\"key-value\",\"DT1-Field-c\":\"field-1c\"}", row1.Json);

            var row2 = actual.AllRows[2];
            Assert.AreEqual("{\"DataType\":\"dt1\",\"key-field\":\"key-value\",\"DT1-Field-c\":\"field-2c\"}", row2.Json);

            var row3 = actual.AllRows[3];
            Assert.AreEqual("{\"DataType\":\"dt2\",\"key-field\":\"field-3b\",\"DT2-Field-c\":\"key-value\",\"DT2-Field-d\":\"field-3d\"}", row3.Json);

            var row4 = actual.AllRows[4];
            Assert.AreEqual("{\"Field-TA\":\"t1\",\"Field-TB\":\"t2\"}", row4.Json);
        }
    }
}
