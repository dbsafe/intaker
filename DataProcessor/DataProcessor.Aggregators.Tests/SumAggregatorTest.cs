using DataProcessor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.Aggregators.Tests
{
    [TestClass]
    public class SumAggregatorTest
    {
        [TestMethod]
        public void AggregateField_Given_a_valid_field_Agregate_shound_be_equal_to_sum_of_the_field_value()
        {
            var aggregate = new Aggregate();
            var target = new SumAggregator { Aggregate = aggregate, Description = "Record Count", Name = "Record Count" };

            var field1 = new Field { Index = 1, Raw = "100", ValidationResult = ValidationResultType.Valid, Value = 100 };
            var field2 = new Field { Index = 2, Raw = "200", ValidationResult = ValidationResultType.Valid, Value = 200 };

            target.AggregateField(field1);
            target.AggregateField(field2);

            Assert.AreEqual(300m, aggregate.Value);
        }
    }
}
