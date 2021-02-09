using DataProcessor.Models;
using DataProcessor.InputDefinitionFile.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace DataProcessor.InputDefinitionFile.Tests.Versions
{
    [TestClass]
    public class InputDefinitionFile20Test
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Serialize()
        {
            var target = new InputDefinitionFile20
            {
                Name = "TestFile",
                Description = "Demo definition file with two data types",
                Version = "1.0",
                FrameworkVersion = "2.0"
            };

            target.Header = new RowDefinition
            {
                Fields = new FieldDefinition[]
                {
                    CreateFieldDefinition("FileType", "Record Type Header", "TextDecoder", "HD", ValidationResultType.Error),
                    CreateFieldDefinition("CreationDate", "Creation Date", "DateDecoder", "MMddyyyy", ValidationResultType.Error)
                }
            };

            var depositRow = new RowDefinition
            {
                DataType = "DP",
                Fields = new FieldDefinition[]
                {
                    CreateFieldDefinition("RecordType", "Record Type Deposit", "TextDecoder", "DP", ValidationResultType.Error),
                    CreateFieldDefinition("ConsumerId", "Consumer Id", "TextDecoder", "[a-zA-Z0-9]{1,16}", ValidationResultType.Error),
                    CreateFieldDefinition("Date", "Date of deposit", "DateDecoder", "MMddyyyy", ValidationResultType.Error),
                    CreateFieldDefinition("Amount", "Amount", "DecimalDecoder", @"(?!0+)([0-9]{1,2},)?([0-9]{1,3},)?([0-9]{1,3},)?[0-9]{1,3}\.[0-9]{2}", ValidationResultType.Error)
                }
            };

            var changeRow = new RowDefinition
            {
                DataType = "CH",
                Fields = new FieldDefinition[]
                {
                    CreateFieldDefinition("RecordType", "Record Type Change", "TextDecoder", "CH", ValidationResultType.Error),
                    CreateFieldDefinition("ConsumerId", "Consumer Id", "TextDecoder", "[a-zA-Z0-9]{1,16}", ValidationResultType.Error),
                    CreateFieldDefinition("Date", "Date of change", "DateDecoder", "MMddyyyy", ValidationResultType.Error),
                    CreateFieldDefinition("AddressLine1", "Street address", "TextDecoder", @"\s*(?:\S\s*){3,100}", ValidationResultType.Error),
                    CreateFieldDefinition("AddressLine2", "Apartment or suite", "TextDecoder", @"\s*(?:\S\s*){3,100}", ValidationResultType.Error)
                }
            };

            target.Datas = new Datas
            {
                KeyField = "ConsumerId",
                DataTypeField = "RecordType",
                Rows = new RowDefinition[] { depositRow, changeRow }
            };

            target.Trailer = new RowDefinition
            {
                Fields = new FieldDefinition[]
                {
                    CreateFieldDefinition("RecordType", "Record Type Tariler", "TextDecoder", "TR", ValidationResultType.Error),
                    CreateFieldDefinition("DepositCount", "Record Count for Deposit", "IntegerDecoder", @"\d{1,5}", ValidationResultType.Error),
                    CreateFieldDefinition("ChangeCount", "Record Count for Change", "IntegerDecoder", @"\d{1,5}", ValidationResultType.Error),
                    CreateFieldDefinition("DepositTotal", "Total deposit amounts", "DecimalDecoder", @"(?!0+)([0-9]{1,2},)?([0-9]{1,3},)?([0-9]{1,3},)?[0-9]{1,3}\.[0-9]{2}", ValidationResultType.Error),
                }
            };

            var xml = HelperXmlSerializer.Serialize(target);
            TestContext.WriteLine(xml);
        }

        [TestMethod]
        public void Deserialize()
        {
            var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(testDirectory, "TestFiles", "FXWDCSV.definition.20.xml");
            var inputXml = File.ReadAllText(path);

            var actual = HelperXmlSerializer.Deserialize<InputDefinitionFile20>(inputXml);
            var outputXml = HelperXmlSerializer.Serialize(actual);
            TestContext.WriteLine(outputXml);

            Assert.AreEqual(inputXml, outputXml);
        }

        private FieldDefinition CreateFieldDefinition(string name, string description, string decoder, string pattern, ValidationResultType failValidationResult)
        {
            return new FieldDefinition
            {
                Name = name,
                Description = description,
                Decoder = decoder,
                Pattern = pattern,
                FailValidationResult = failValidationResult
            };
        }
    }
}
