using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataProcessor.Rules.Tests
{
    [TestClass]
    public class MatchesRowCountRuleTest
    {
        [TestMethod]
        public void Validate_Given_field_value_matches_the_aggregate_ValidResult_should_be_valid()
        {
            var aggregates = new Aggregate[]
            {
                new Aggregate {Name = "aggregate-1", Value = 10},
                new Aggregate {Name = "aggregate-2", Value = 20}
            };

            var target = new MatchesRowCountRule("rule-name", "rule-description", "{'ruleValue':'aggregate-2'}", ValidationResultType.InvalidFixable);
            target.SetAggregates(aggregates);

            var field = new Field { Value = "20", ValidationResult = ValidationResultType.Valid };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Validate_Given_field_value_does_not_matche_the_aggregate_ValidResult_should_be_invalid()
        {
            var aggregates = new Aggregate[]
            {
                new Aggregate {Name = "aggregate-1", Value = 10},
                new Aggregate {Name = "aggregate-2", Value = 20}
            };

            var target = new MatchesRowCountRule("rule-name", "rule-description", "{'ruleValue':'aggregate-2'}", ValidationResultType.InvalidCritical);
            target.SetAggregates(aggregates);

            var field = new Field { Value = "5", ValidationResult = ValidationResultType.Valid };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.InvalidCritical, field.ValidationResult);
        }

        [TestMethod]
        public void SetAggregates_Given_that_the_aggregator_is_not_found_Should_throw_an_exception()
        {
            var aggregates = new Aggregate[]
            {
                new Aggregate {Name = "aggregate-1", Value = 10},
                new Aggregate {Name = "aggregate-2", Value = 20}
            };

            var target = new MatchesRowCountRule("rule-name", "rule-description", "{'ruleValue':'aggregate-3'}", ValidationResultType.InvalidCritical);

            try
            {
                target.SetAggregates(aggregates);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual($"DataProcessor.Rules.MatchesRowCountRule - Aggregate 'aggregate-3' not found", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");

        }
    }
}
