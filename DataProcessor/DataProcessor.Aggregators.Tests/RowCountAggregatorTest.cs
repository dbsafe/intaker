using DataProcessor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.Aggregators.Tests
{
    [TestClass]
    public class RowCountAggregatorTest
    {
        [TestMethod]
        public void AggregateField_Given_a_valid_field_Agregate_shound_be_equal_to_the_number_of_calls()
        {
            var aggregate = new Aggregate();
            var target = new RowCountAggregator { Aggregate = aggregate, Description = "Row Count", Name = "RowCount" };

            var field = new Field { Raw = "abc", ValidationResult = ValidationResultType.Valid, Value = "abc" };

            target.AggregateField(field);

            Assert.AreEqual(1, aggregate.Value);

            target.AggregateField(field);

            Assert.AreEqual(2, aggregate.Value);
        }

        [TestMethod]
        public void AggregateField_Given_an_invalid_field_Agregate_shound_be_equal_to_the_number_of_calls()
        {
            var aggregate = new Aggregate();
            var target = new RowCountAggregator { Aggregate = aggregate, Description = "Row Count", Name = "RowCount" };

            var field = new Field { Raw = "abc", ValidationResult = ValidationResultType.Critical, Value = "abc" };

            target.AggregateField(field);

            Assert.AreEqual(1, aggregate.Value);

            target.AggregateField(field);

            Assert.AreEqual(2, aggregate.Value);
        }
    }
}
