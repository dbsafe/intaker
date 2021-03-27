using DataProcessor.Aggregators;
using DataProcessor.Decoders;
using DataProcessor.InputDefinitionFile;
using DataProcessor.Models;
using DataProcessor.Rules;
using DataProcessor.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DataProcessor.ProcessorDefinition.Tests
{
    [TestClass]
    public class ProcessorDefinitionBuilder20Test : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DataProcessorGlobal.IsDebugEnabled = true;
            PrintLoadedAssemblies();
        }

        [TestMethod]
        public void CreateProcessorDefinition20_Given_an_input_definition_file_Decoders_should_be_created()
        {
            var inputDefinitionFile = BuildInputDefinitionFile20();

            var actual = FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            Assert.IsTrue(actual.CreateRowJsonEnabled);

            // Header definition
            Assert.IsNotNull(actual.HeaderRowProcessorDefinition);
            Assert.IsNotNull(actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(5, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions.Length);
            AssertFieldProcessorDefinition("RecordType", "HD", typeof(TextDecoder), ValidationResultType.Error, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[0]);
            AssertFieldProcessorDefinition("CreationDate", "MMddyyyy", typeof(DateDecoder), ValidationResultType.Error, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[1]);
            AssertFieldProcessorDefinition("LocationID", "[a-zA-Z]{12}", typeof(TextDecoder), ValidationResultType.Error, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[2]);
            AssertFieldProcessorDefinition("SequenceNumber", "(?!0{4})[0-9]{4}", typeof(IntegerDecoder), ValidationResultType.Error, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[3]);
            AssertFieldProcessorDefinition("Optional", null, typeof(BypassDecoder), ValidationResultType.Valid, actual.HeaderRowProcessorDefinition.FieldProcessorDefinitions[4]);

            // Data definition
            Assert.IsNotNull(actual.DataRowProcessorDefinitions);
            Assert.AreEqual(2, actual.DataRowProcessorDefinitions.Keys.Count);

            Assert.IsTrue(actual.DataRowProcessorDefinitions.ContainsKey("BL"));
            {
                var dataBL = actual.DataRowProcessorDefinitions["BL"];
                Assert.AreEqual(7, dataBL.RowProcessorDefinition.FieldProcessorDefinitions.Length);
                AssertFieldProcessorDefinition("RecordType", "BL", typeof(TextDecoder), ValidationResultType.Error, dataBL.RowProcessorDefinition.FieldProcessorDefinitions[0]);
                AssertFieldProcessorDefinition("ConsumerID", "[0-9]{1,10}", typeof(IntegerDecoder), ValidationResultType.Error, dataBL.RowProcessorDefinition.FieldProcessorDefinitions[1]);
                AssertFieldProcessorDefinition("SSN", @"\d{3}-\d{2}-\d{4}", typeof(TextDecoder), ValidationResultType.Error, dataBL.RowProcessorDefinition.FieldProcessorDefinitions[2]);
                AssertFieldProcessorDefinition("FirstName", @"[a-zA-Z0-9\s-']{2,35}", typeof(TextDecoder), ValidationResultType.Error, dataBL.RowProcessorDefinition.FieldProcessorDefinitions[3]);
                AssertFieldProcessorDefinition("LastName", @"[a-zA-Z0-9\s-']{2,35}", typeof(TextDecoder), ValidationResultType.Error, dataBL.RowProcessorDefinition.FieldProcessorDefinitions[4]);
                AssertFieldProcessorDefinition("DOB", "MMddyyyy", typeof(DateDecoder), ValidationResultType.Error, dataBL.RowProcessorDefinition.FieldProcessorDefinitions[5]);
                AssertFieldProcessorDefinition("Balance", @"-{0,1}[0-9]{1,10}\.[0-9]{2}", typeof(DecimalDecoder), ValidationResultType.Error, dataBL.RowProcessorDefinition.FieldProcessorDefinitions[6]);
            }

            Assert.IsTrue(actual.DataRowProcessorDefinitions.ContainsKey("CH"));
            {
                var dataCH = actual.DataRowProcessorDefinitions["CH"];
                Assert.AreEqual(5, dataCH.RowProcessorDefinition.FieldProcessorDefinitions.Length);
                AssertFieldProcessorDefinition("RecordType", "CH", typeof(TextDecoder), ValidationResultType.Error, dataCH.RowProcessorDefinition.FieldProcessorDefinitions[0]);
                AssertFieldProcessorDefinition("ConsumerID", "[0-9]{1,10}", typeof(IntegerDecoder), ValidationResultType.Error, dataCH.RowProcessorDefinition.FieldProcessorDefinitions[1]);
                AssertFieldProcessorDefinition("Date", "MMddyyyy", typeof(DateDecoder), ValidationResultType.Error, dataCH.RowProcessorDefinition.FieldProcessorDefinitions[2]);
                AssertFieldProcessorDefinition("AddressLine1", @"\s*(?:\S\s*){3,100}", typeof(TextDecoder), ValidationResultType.Error, dataCH.RowProcessorDefinition.FieldProcessorDefinitions[3]);
                AssertFieldProcessorDefinition("AddressLine2", @"\s*(?:\S\s*){3,100}", typeof(TextDecoder), ValidationResultType.Error, dataCH.RowProcessorDefinition.FieldProcessorDefinitions[4]);
            }

            // Trailer definition
            Assert.IsNotNull(actual.TrailerRowProcessorDefinition);
            Assert.IsNotNull(actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(3, actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions.Length);
            AssertFieldProcessorDefinition("RecordType", "TR", typeof(TextDecoder), ValidationResultType.Error, actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions[0]);
            AssertFieldProcessorDefinition("BalanceTotal", @"-{0,1}[0-9]{1,10}\.[0-9]{2}", typeof(DecimalDecoder), ValidationResultType.Error, actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions[1]);
            AssertFieldProcessorDefinition("RecordCount", @"\d{1,5}", typeof(IntegerDecoder), ValidationResultType.Error, actual.TrailerRowProcessorDefinition.FieldProcessorDefinitions[2]);
        }

        [TestMethod]
        public void CreateProcessorDefinition20_Given_an_input_definition_file_Rules_should_be_created()
        {
            var inputDefinitionFile = BuildInputDefinitionFile20();

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
        public void CreateProcessorDefinition20_Given_an_input_definition_file_Aggregators_should_be_created()
        {
            var inputDefinitionFile = BuildInputDefinitionFile20();

            var actual = FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            Assert.IsNotNull(actual.DataRowProcessorDefinitions);
            Assert.IsTrue(actual.DataRowProcessorDefinitions.ContainsKey("BL"));

            var dataBL = actual.DataRowProcessorDefinitions["BL"];
            Assert.IsNotNull(dataBL.RowProcessorDefinition.FieldProcessorDefinitions);
            Assert.AreEqual(7, dataBL.RowProcessorDefinition.FieldProcessorDefinitions.Length);
            Assert.AreEqual("Balance", dataBL.RowProcessorDefinition.FieldProcessorDefinitions[6].FieldName);

            var aggregators = actual.DataRowProcessorDefinitions["BL"].RowProcessorDefinition.FieldProcessorDefinitions[6].Aggregators;
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

        [TestMethod]
        public void CreateProcessorDefinition20_Given_an_input_definition_file_KeyField_should_be_set()
        {
            var inputDefinitionFile = BuildInputDefinitionFile20();

            var actual = FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            Assert.AreEqual("ConsumerID", actual.KeyField);
        }

        [TestMethod]
        public void CreateProcessorDefinition20_Given_an_input_definition_file_DataTypeFieldIndex_should_be_set()
        {
            var inputDefinitionFile = BuildInputDefinitionFile20();

            var actual = FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            Assert.AreEqual("RecordType", actual.DataTypeField);
            Assert.AreEqual(0, actual.DataRowProcessorDefinitions["BL"].DataTypeFieldIndex);
            Assert.AreEqual(0, actual.DataRowProcessorDefinitions["CH"].DataTypeFieldIndex);
        }

        [TestMethod]
        public void CreateProcessorDefinition20_Given_an_input_definition_file_DataKeyFieldIndex_should_be_set()
        {
            var inputDefinitionFile = BuildInputDefinitionFile20();

            var actual = FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            Assert.AreEqual("RecordType", actual.DataTypeField);
            Assert.AreEqual(1, actual.DataRowProcessorDefinitions["BL"].DataKeyFieldIndex);
            Assert.AreEqual(1, actual.DataRowProcessorDefinitions["CH"].DataKeyFieldIndex);
        }

        [TestMethod]
        public void CreateProcessorDefinition20_Given_an_input_definition_with_invalid_file_KeyField_should_throw_an_exception()
        {
            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer-invalid-keyField.definition.20.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile20>(path);

            try
            {
                FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);
                Assert.Fail("An exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual($"KeyField 'test-key-field' must be present in every data definition", ex.Message);
            }
        }

        [TestMethod]
        public void CreateProcessorDefinition20_Given_an_input_definition_with_invalid_file_DataTypeField_should_throw_an_exception()
        {
            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer-invalid-dataTypeField.definition.20.xml");
            var inputDefinitionFile = FileLoader.Load<InputDefinitionFile20>(path);

            try
            {
                FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);
                Assert.Fail("An exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual($"DataTypeField 'test-data-type-field' must be present in every data definition", ex.Message);
            }
        }

        private InputDefinitionFile20 BuildInputDefinitionFile20()
        {
            var path = Path.Combine(_testDirectory, "TestFiles", "balance-with-header-and-trailer.definition.20.xml");
            return FileLoader.Load<InputDefinitionFile20>(path);
        }
    }
}
