using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.Aggregators.Tests
{
    [TestClass]
    public class RecordCountAggregatorTest
    {
        [TestMethod]
        public void AggregateField_Given_a_valid_field_Agregate_shound_be_equal_to_the_number_of_calls()
        {
            var aggregate = new Aggregate();
            var target = new RecordCountAggregator { Aggregate = aggregate, Description = "Record Count", Name = "Record Count" };

            var field = new Field { Raw = "abc", ValidationResult = ValidationResultType.Valid, Value = "abc" };

            target.AggregateField(field);

            Assert.AreEqual(1, aggregate.Value);

            target.AggregateField(field);

            Assert.AreEqual(2, aggregate.Value);
        }

        [TestMethod]
        public void AggregateField_Given_a_invalid_field_Agregate_shound_be_equal_to_the_number_of_calls()
        {
            var aggregate = new Aggregate();
            var target = new RecordCountAggregator { Aggregate = aggregate, Description = "Record Count", Name = "Record Count" };

            var field = new Field { Raw = "abc", ValidationResult = ValidationResultType.InvalidCritical, Value = "abc" };

            target.AggregateField(field);

            Assert.AreEqual(1, aggregate.Value);

            target.AggregateField(field);

            Assert.AreEqual(2, aggregate.Value);
        }
    }
}
