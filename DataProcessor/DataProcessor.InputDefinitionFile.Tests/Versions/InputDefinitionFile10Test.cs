using DataProcessor.Models;
using DataProcessor.InputDefinitionFile.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace DataProcessor.InputDefinitionFile.Tests.Versions
{
    [TestClass]
    public class InputDefinitionFile10Test
    {
        private readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Serialize()
        {
            var sequenceNumberFieldDefinition = CreateFieldDefinition("SequenceNumber", "Sequence Number", "IntegerDecoder", "(?!0{4})[0-9]{4}", ValidationResultType.Error);

            var minNumberFieldRule = new RuleDefinition
            {
                Name = "SequenceNumber-MinNumberFieldRule",
                Description = "Sequence number should be greater or equal to 10",
                Rule = "MinNumberFieldRule",
                SingleArg = "10",
                FailValidationResult = ValidationResultType.Error
            };

            var maxNumberFieldRule = new RuleDefinition
            {
                Name = "SequenceNumber-MaxNumberFieldRule",
                Description = "Sequence number should be equal or less than 100",
                Rule = "MinNumberFieldRule",
                Args = new ArgDefinition[] { new ArgDefinition { Name = "ruleValue", Value = "100" } },
                FailValidationResult = ValidationResultType.Error
            };

            sequenceNumberFieldDefinition.Rules = new RuleDefinition[] { minNumberFieldRule, maxNumberFieldRule };

            var originatorName = CreateFieldDefinition("OriginatorName", "Originator Name", "TextDecoder", @"[a-zA-Z\s-']{2,35}", ValidationResultType.Error);
            originatorName.Aggregators = new AggregatorDefinition[]
            {
                new AggregatorDefinition("BalanceAggregator", "Balance aggregator", "SumAggregator")
            };

            var target = new InputDefinitionFile10
            {
                Name = "FXWDCSV",
                Description = "Demo defining part of FXWDCSV",
                Version = "1.0",
                FrameworkVersion = "1.0"
            };

            target.Header = new RowDefinition
            {
                Fields = new FieldDefinition[]
                {
                    CreateFieldDefinition("FileType", "File Type", "TextDecoder", "PAYMENT", ValidationResultType.Error),
                    CreateFieldDefinition("RecordType", "Record Type", "TextDecoder", "HEADER", ValidationResultType.Error),
                    CreateFieldDefinition("CreationDate", "Creation Date", "DateDecoder", "MMddyyyy", ValidationResultType.Error),
                    CreateFieldDefinition("LocationID", "Location ID", "TextDecoder", "[a-zA-Z]{12}", ValidationResultType.Warning),
                    sequenceNumberFieldDefinition
                }
            };

            target.Data = new RowDefinition
            {
                Fields = new FieldDefinition[]
                {
                    CreateFieldDefinition("RecordType", "Record Type", "TextDecoder", "PAYMENT", ValidationResultType.Error),
                    CreateFieldDefinition("PaymentType", "Payment Type", "TextDecoder", "(FXW|MBW)", ValidationResultType.Error),
                    CreateFieldDefinition("SendersReferenceNumber", "Senders Reference Number", "TextDecoder", "[a-zA-Z0-9]{1,16}", ValidationResultType.Error),
                    CreateFieldDefinition("RelatedReferenceNumber", "Related Reference Number", "TextDecoder", "[a-zA-Z0-9]{1,16}", ValidationResultType.Error),
                    CreateFieldDefinition("ValueDate", "Value Date", "DateDecoder", "MMddyyyy", ValidationResultType.Error),
                    CreateFieldDefinition("PaymentType", "Payment Type", "TextDecoder", "(DEBIT|CREDIT)", ValidationResultType.Error),
                    CreateFieldDefinition("Amount", "Amount", "DecimalDecoder", @"(?!0+)([0-9]{1,2},)?([0-9]{1,3},)?([0-9]{1,3},)?[0-9]{1,3}\.[0-9]{2}", ValidationResultType.Error),
                    CreateFieldDefinition("CreditCurrency", "Credit Currency", "TextDecoder", "[A-Z]{3}", ValidationResultType.Error),
                    originatorName
                }
            };

            target.Trailer = new RowDefinition
            {
                Fields = new FieldDefinition[]
                {
                    CreateFieldDefinition("FileType", "File Type", "TextDecoder", "PAYMENT", ValidationResultType.Error),
                    CreateFieldDefinition("RecordType", "Record Type", "TextDecoder", "TRAILER", ValidationResultType.Error),
                    CreateFieldDefinition("HashTotal", "Hash Total", "DecimalDecoder", @"(?!0+)([0-9]{1,2},)?([0-9]{1,3},)?([0-9]{1,3},)?[0-9]{1,3}\.[0-9]{2}", ValidationResultType.Error),
                    CreateFieldDefinition("RecordCount", "Record Count", "IntegerDecoder", @"\d{1,5}", ValidationResultType.Error)
                }
            };

            var xml = HelperXmlSerializer.Serialize(target);
            TestContext.WriteLine(xml);
        }

        [TestMethod]
        public void Deserialize()
        {
            var inputXml = LoadInputXml10();

            var actual = HelperXmlSerializer.Deserialize<InputDefinitionFile10>(inputXml);
            var outputXml = HelperXmlSerializer.Serialize(actual);
            TestContext.WriteLine(outputXml);

            Assert.AreEqual(inputXml, outputXml);
        }

        [TestMethod]
        public void Deserialize_Given_an_input_file_that_uses_default_values_Should_deserialise()
        {
            var inputXml = LoadInputXml10();
            var path = Path.Combine(_testDirectory, "TestFiles", "FXWDCSV-default-values.definition.xml");
            var inputXmlWithDefaultValues = File.ReadAllText(path);

            var actual = HelperXmlSerializer.Deserialize<InputDefinitionFile10>(inputXmlWithDefaultValues);
            var outputXml = HelperXmlSerializer.Serialize(actual);
            TestContext.WriteLine(outputXml);

            Assert.AreEqual(inputXml, outputXml);
        }

        private string LoadInputXml10()
        {
            var path = Path.Combine(_testDirectory, "TestFiles", "FXWDCSV.definition.10.xml");
            return File.ReadAllText(path);
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
