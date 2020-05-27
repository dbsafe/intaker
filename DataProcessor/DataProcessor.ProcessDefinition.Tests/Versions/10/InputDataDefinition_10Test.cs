using DataProcessor.ProcessDefinition.Models;
using DataProcessor.ProcessDefinition.Utils;
using DataProcessor.ProcessDefinition.Versions_10;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace DataProcessor.ProcessDefinition.Tests.Versions_10
{
    [TestClass]
    public class InputDataDefinition_10Test
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Serialize()
        {
            var target = new InputDataDefinition_10
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
                    new FieldDefinition("FileType", "File Type", "TextDecoder", "PAYMENT"),
                    new FieldDefinition("RecordType", "Record Type", "TextDecoder", "HEADER"),
                    new FieldDefinition("CreationDate", "Creation Date", "DateDecoder", "MMddyyyy"),
                    new FieldDefinition("LocationID", "Location ID", "TextDecoder", "[a-zA-Z]{12}"),
                    new FieldDefinition("SequenceNumber", "Sequence Number", "NumberDecoder", "(?!0{4})[0-9]{4}")
                    {
                        Rules = new RuleDefinition[]
                        {
                            new RuleDefinition("SequenceNumber-MinNumberFieldRule", "Sequence number should be greater or equal to 10", "MinNumberFieldRule", "{'min':'10'}")
                        }
                    }
                }
            };

            target.Data = new RowDefinition
            {
                Fields = new FieldDefinition[]
                {
                    new FieldDefinition("RecordType", "Record Type", "TextDecoder", "PAYMENT"),
                    new FieldDefinition("PaymentType", "Payment Type", "TextDecoder", "(FXW|MBW)"),
                    new FieldDefinition("SendersReferenceNumber", "Senders Reference Number", "TextDecoder", "[a-zA-Z0-9]{1,16}"),
                    new FieldDefinition("RelatedReferenceNumber", "Related Reference Number", "TextDecoder", "[a-zA-Z0-9]{1,16}"),
                    new FieldDefinition("ValueDate", "Value Date", "DateDecoder", "MMddyyyy"),
                    new FieldDefinition("PaymentType", "Payment Type", "TextDecoder", "(DEBIT|CREDIT)"),
                    new FieldDefinition("Amount", "Amount", "NumberDecoder", @"(?!0+)([0-9]{1,2},)?([0-9]{1,3},)?([0-9]{1,3},)?[0-9]{1,3}\.[0-9]{2}"),
                    new FieldDefinition("CreditCurrency", "Credit Currency", "TextDecoder", "[A-Z]{3}"),
                    new FieldDefinition("OriginatorName", "Originator Name", "TextDecoder", @"[a-zA-Z\s-']{2,35}")
                    {
                        Aggregators = new AggregatorDefinition[]
                        {
                            new AggregatorDefinition("BalanceAggregator", "Balance aggregator", "SumAggregator")
                        }
                    }
                }
            };

            target.Trailer = new RowDefinition
            {
                Fields = new FieldDefinition[]
                {
                    new FieldDefinition("FileType", "File Type", "TextDecoder", "PAYMENT"),
                    new FieldDefinition("RecordType", "Record Type", "TextDecoder", "TRAILER"),
                    new FieldDefinition("HashTotal", "Hash Total", "NumberDecoder", @"(?!0+)([0-9]{1,2},)?([0-9]{1,3},)?([0-9]{1,3},)?[0-9]{1,3}\.[0-9]{2}"),
                    new FieldDefinition("RecordCount", "Record Count", "NumberDecoder", @"\d{1,5}")
                }
            };

            var xml = HelperXmlSerializer.Serialize(target);
            TestContext.WriteLine(xml);
        }

        [TestMethod]
        public void Deserialize()
        {
            var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(testDirectory, "TestFiles", "FXWDCSV.definition.xml");
            var inputXml = File.ReadAllText(path);

            var actual = HelperXmlSerializer.Deserialize<InputDataDefinition_10>(inputXml);
            var outputXml = HelperXmlSerializer.Serialize(actual);
            TestContext.WriteLine(outputXml);

            Assert.AreEqual(inputXml, outputXml);
        }
    }
}
