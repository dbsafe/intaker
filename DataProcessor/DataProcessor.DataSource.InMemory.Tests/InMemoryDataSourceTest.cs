using DataProcessor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DataProcessor.DataSource.InMemory.Tests
{
    [TestClass]
    public class InMemoryDataSourceTest
    {
        [TestMethod]
        public void Process_Given_a_file_ProcessField_event_should_be_raised_for_every_field()
        {
            var target = CreateTarget(true);
            var context = new ParserContext();

            var expected = new List<Field>
            {
                new Field { Index = 0, Raw = "field-1a", ValidationResult = ValidationResultType.Valid, Value = "1a" },
                new Field { Index = 1, Raw = "field-1b", ValidationResult = ValidationResultType.Valid, Value = "1b" },

                new Field { Index = 0, Raw = "field-2a", ValidationResult = ValidationResultType.Valid, Value = "2a" },
                new Field { Index = 1, Raw = "field-2b", ValidationResult = ValidationResultType.Valid, Value = "2b" },
                new Field { Index = 2, Raw = "field-2c", ValidationResult = ValidationResultType.Valid, Value = "2c" },

                new Field { Index = 0, Raw = "field-3a", ValidationResult = ValidationResultType.Valid, Value = "3a" },
                new Field { Index = 1, Raw = "field-3b", ValidationResult = ValidationResultType.Valid, Value = "3b" }
            };

            var actual = new List<Field>();
            target.ProcessField += (sender, e) =>
            {
                e.Field.Value = e.Field.Raw[6..];
                actual.Add(e.Field);
            };

            target.Process(context);

            AssertFields(expected, actual);
        }

        private InMemoryDataSource<ParserContext> CreateTarget(bool hasFieldsEnclosedInQuotes)
        {
            var config = new InMemoryDataSourceConfig
            {
                Delimiter = ",",
                HasFieldsEnclosedInQuotes = hasFieldsEnclosedInQuotes
            };

            var lines = new string[]
            {
                "\"field-1a\",\"field-1b\"",
                "\"field-2a\",\"field-2b\",\"field-2c\"",
                "\"field-3a\",\"field-3b\""
            };

            return new InMemoryDataSource<ParserContext>(config, lines);
        }

        private void AssertFields(IList<Field> expected, IList<Field> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                AssertField(expected[i], actual[i]);
            }
        }

        private void AssertField(Field expected, Field actual)
        {
            Assert.AreEqual(expected.Index, actual.Index);
            Assert.AreEqual(expected.Raw, actual.Raw);
            Assert.AreEqual(expected.ValidationResult, actual.ValidationResult);
            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}
