using DataProcessor.Aggregators;
using DataProcessor.Decoders;
using DataProcessor.InputDefinitionFile;
using DataProcessor.Models;
using DataProcessor.Rules;
using DataProcessor.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DataProcessor.ProcessorDefinition.Tests
{
    [TestClass]
    public class ProcessorDefinitionBuilder10Test : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DataProcessorGlobal.IsDebugEnabled = true;
            PrintLoadedAssemblies();
        }

        [TestMethod]
        public void CreateProcessorDefinition10_Given_an_input_definition_file_Decoders_should_be_created()
        {
            var inputDefinitionFile = BuildInputDefinitionFile10();

            var actual = FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            Assert.IsTrue(actual.CreateRowJsonEnabled);

            // Header definition
            Assert.IsNotNull(actual.HeaderRowProcessorDefinition);
            Assert.IsNotNull(actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(5, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions.Length);
            AssertFieldProcessorDefinition("RecordType", "HEADER", typeof(TextDecoder), ValidationResultType.Error, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[0]);
            AssertFieldProcessorDefinition("CreationDate", "MMddyyyy", typeof(DateDecoder), ValidationResultType.Error, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[1]);
            AssertFieldProcessorDefinition("LocationID", "[a-zA-Z]{12}", typeof(TextDecoder), ValidationResultType.Error, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[2]);
            AssertFieldProcessorDefinition("SequenceNumber", "(?!0{4})[0-9]{4}", typeof(IntegerDecoder), ValidationResultType.Error, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[3]);
            AssertFieldProcessorDefinition("Optional", null, typeof(BypassDecoder), ValidationResultType.Valid, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[4]);

            // Data definition
            Assert.IsNotNull(actual.DataRowProcessorDefinition);
            Assert.IsNotNull(actual.DataRowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(7, actual.DataRowProcessorDefinition.FieldProcessorDefinitions.Length);
            AssertFieldProcessorDefinition("RecordType", "BALANCE", typeof(TextDecoder), ValidationResultType.Error, actual.DataRowProcessorDefinition.FieldProcessorDefinitions[0]);
            AssertFieldProcessorDefinition("ConsumerID", "[0-9]{1,10}", typeof(IntegerDecoder), ValidationResultType.Error, actual.DataRowProcessorDefinition.FieldProcessorDefinitions[1]);
            AssertFieldProcessorDefinition("SSN", @"\d{3}-\d{2}-\d{4}", typeof(TextDecoder), ValidationResultType.Error, actual.DataRowProcessorDefinition.FieldProcessorDefinitions[2]);
            AssertFieldProcessorDefinition("FirstName", @"[a-zA-Z0-9\s-']{2,35}", typeof(TextDecoder), ValidationResultType.Error, actual.DataRowProcessorDefinition.FieldProcessorDefinitions[3]);
            AssertFieldProcessorDefinition("LastName", @"[a-zA-Z0-9\s-']{2,35}", typeof(TextDecoder), ValidationResultType.Error, actual.DataRowProcessorDefinition.FieldProcessorDefinitions[4]);
            AssertFieldProcessorDefinition("DOB", "MMddyyyy", typeof(DateDecoder), ValidationResultType.Error, actual.DataRowProcessorDefinition.FieldProcessorDefinitions[5]);
            AssertFieldProcessorDefinition("Balance", @"-{0,1}[0-9]{1,10}\.[0-9]{2}", typeof(DecimalDecoder), ValidationResultType.Error, actual.DataRowProcessorDefinition.FieldProcessorDefinitions[6]);

            // Trailer definition
            Assert.IsNotNull(actual.TrailerRowProcessorDefinition);
            Assert.IsNotNull(actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(3, actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions.Length);
            AssertFieldProcessorDefinition("RecordType", "TRAILER", typeof(TextDecoder), ValidationResultType.Error, actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions[0]);
            AssertFieldProcessorDefinition("BalanceTotal", @"-{0,1}[0-9]{1,10}\.[0-9]{2}", typeof(DecimalDecoder), ValidationResultType.Error, actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions[1]);
            AssertFieldProcessorDefinition("RecordCount", @"\d{1,5}", typeof(IntegerDecoder), ValidationResultType.Error, actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions[2]);
        }

        [TestMethod]
        public void CreateProcessorDefinition10_Given_an_input_definition_file_Rules_should_be_created()
        {
            var inputDefinitionFile = BuildInputDefinitionFile10();

            var actual = FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            Assert.IsNotNull(actual.HeaderRowProcessorDefinition);
            Assert.IsNotNull(actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(5, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions.Length);
            Assert.AreEqual("SequenceNumber", actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[3].FieldName);

            var rules = actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[3].Rules;
            Assert.IsNotNull(rules);
            Assert.AreEqual(2, rules.Length);

            var rule0 = rules[0];
            Assert.AreEqual("NumericValue", rule0.Args[0].Key);
            Assert.AreEqual("10", rule0.Args[0].Value);
            Assert.AreEqual("Minimum sequence number should be 10", rule0.Description);
            Assert.AreEqual(ValidationResultType.Warning, rule0.FailValidationResult);
            Assert.AreEqual(typeof(MinNumberFieldRule), rule0.GetType());
            Assert.AreEqual("SequenceNumber-MinNumberFieldRule", rule0.Name);

            var rule1 = rules[1];
            Assert.AreEqual("NumericValue", rule1.Args[0].Key);
            Assert.AreEqual("100", rule1.Args[0].Value);
            Assert.AreEqual("Maximum sequence number should be 100", rule1.Description);
            Assert.AreEqual(ValidationResultType.Error, rule1.FailValidationResult);
            Assert.AreEqual(typeof(MaxNumberFieldRule), rule1.GetType());
            Assert.AreEqual("SequenceNumber-MaxNumberFieldRule", rule1.Name);
        }

        [TestMethod]
        public void CreateProcessorDefinition10_Given_an_input_definition_file_Aggregators_should_be_created()
        {
            var inputDefinitionFile = BuildInputDefinitionFile10();

            var actual = FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            Assert.IsNotNull(actual.DataRowProcessorDefinition);
            Assert.IsNotNull(actual.DataRowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(7, actual.DataRowProcessorDefinition.FieldProcessorDefinitions.Length);
            Assert.AreEqual("Balance", actual.DataRowProcessorDefinition.FieldProcessorDefinitions[6].FieldName);

            var aggregators = actual.DataRowProcessorDefinition.FieldProcessorDefinitions[6].Aggregators;
            Assert.IsNotNull(aggregators);
            Assert.AreEqual(2, aggregators.Length);

            var aggregator0 = aggregators[0];
            Assert.AreEqual("Balance aggregator", aggregator0.Description);
            Assert.AreEqual(typeof(SumAggregator), aggregator0.GetType());
            Assert.AreEqual("BalanceAggregator", aggregator0.Name);

            var aggregator1 = aggregators[1];
            Assert.AreEqual("Data row counter", aggregator1.Description);
            Assert.AreEqual(typeof(RowCountAggregator), aggregator1.GetType());
            Assert.AreEqual("DataRowCountAggregator", aggregator1.Name);
        }

        private InputDefinitionFile10 BuildInputDefinitionFile10()
        {
            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer.definition.10.xml");
            return FileLoader.Load<InputDefinitionFile10>(path);
        }
    }
}
