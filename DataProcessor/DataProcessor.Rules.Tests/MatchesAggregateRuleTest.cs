using DataProcessor.Contracts;
using DataProcessor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DataProcessor.Rules.Tests
{
    [TestClass]
    public class MatchesAggregateRuleTest
    {
        private readonly FieldRuleConfiguration _config = new FieldRuleConfiguration
        {
            Aggregates = new Aggregate[]
            {
                new Aggregate {Name = "aggregate-1", Value = 10},
                new Aggregate {Name = "aggregate-2", Value = 20}
            }
        };

        [TestMethod]
        public void Validate_Given_field_value_matches_the_aggregate_ValidResult_should_be_valid()
        {
            var config = new FieldRuleConfiguration
            {
                Aggregates = new Aggregate[]
                {
                    new Aggregate {Name = "aggregate-1", Value = 10},
                    new Aggregate {Name = "aggregate-2", Value = 20}
                }
            };

            var target = CreateRule("rule-name", "rule-description", "aggregate-2", ValidationResultType.Warning);
            target.Initialize(config);

            var field = new Field { Value = 20, ValidationResult = ValidationResultType.Valid };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Validate_Given_that_property_failValidationResult_is_not_set_Should_throw_an_exception()
        {
            var config = new FieldRuleConfiguration
            {
                Aggregates = new Aggregate[]
                {
                    new Aggregate {Name = "aggregate-1", Value = 10},
                    new Aggregate {Name = "aggregate-2", Value = 20}
                }
            };

            var target = new MatchesAggregateRule { Description = "rule-description", Name = "rule-name" };
            try
            {
                target.Initialize(config);
                Assert.Fail("An exception was not thrown");
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("Property FailValidationResult must be set", e.Message);
            }
        }

        [TestMethod]
        public void Validate_Given_field_value_does_not_match_the_aggregate_ValidResult_should_be_invalid()
        {
            var target = CreateRule("rule-name", "rule-description", "aggregate-2", ValidationResultType.Critical);
            target.Initialize(_config);

            var field = new Field { Value = "5", ValidationResult = ValidationResultType.Valid };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Critical, field.ValidationResult);
        }

        [TestMethod]
        public void Initialize_Given_that_the_aggregator_is_not_found_Should_throw_an_exception()
        {
            var target = CreateRule("rule-name", "rule-description", "aggregate-3", ValidationResultType.Critical);
            try
            {
                target.Initialize(_config);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual($"Rule: 'rule-name'. Aggregate 'aggregate-3' not found", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void Initialize_Given_an_invalid_aggregator_name_Should_throw_an_exception()
        {
            try
            {
                var target = CreateRule("rule-name", "rule-description", "aggregate-100", ValidationResultType.Critical);
                target.Initialize(_config);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Rule: 'rule-name'. Aggregate 'aggregate-100' not found", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void Initialize_Given_missing_aggregate_arg_Should_throw_an_exception()
        {
            try
            {
                var target = CreateRuleWithoutArgs("rule-name", "rule-description", ValidationResultType.Critical);
                target.Initialize(_config);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Rule: 'rule-name'. Argument 'AggregateName' not found", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        public MatchesAggregateRule CreateRule(string name, string description, string argAgregatorValue, ValidationResultType failValidationResult)
        {
            return new MatchesAggregateRule
            {
                Description = description,
                FailValidationResult = failValidationResult,
                Name = name,
                Args = new KeyValuePair<string, string>[] 
                { 
                    new KeyValuePair<string, string>("AggregateName", argAgregatorValue) 
                }
            };
        }

        public MatchesAggregateRule CreateRuleWithoutArgs(string name, string description, ValidationResultType failValidationResult)
        {
            return new MatchesAggregateRule
            {
                Description = description,
                FailValidationResult = failValidationResult,
                Name = name
            };
        }
    }
}
