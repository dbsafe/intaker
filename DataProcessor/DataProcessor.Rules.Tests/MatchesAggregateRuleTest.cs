using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

            var target = CreateRule("rule-name", "rule-description", "{'ruleValue':'aggregate-2'}", ValidationResultType.Warning);
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

            var target = new MatchesAggregateRule { Description = "rule-description", Name = "rule-name", Args = "{'ruleValue':'aggregate-2'}" };


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
        public void Validate_Given_field_value_does_not_matche_the_aggregate_ValidResult_should_be_invalid()
        {
            var target = CreateRule("rule-name", "rule-description", "{'ruleValue':'aggregate-2'}", ValidationResultType.Critical);
            target.Initialize(_config);

            var field = new Field { Value = "5", ValidationResult = ValidationResultType.Valid };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Critical, field.ValidationResult);
        }

        [TestMethod]
        public void Initialize_Given_that_the_aggregator_is_not_found_Should_throw_an_exception()
        {
            var target = CreateRule("rule-name", "rule-description", "{'ruleValue':'aggregate-3'}", ValidationResultType.Critical);

            try
            {
                target.Initialize(_config);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual($"DataProcessor.Rules.MatchesAggregateRule - Aggregate 'aggregate-3' not found", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void Initialize_Given_an_invalid_args_Should_throw_an_exception()
        {
            var target = CreateRule("rule-name", "rule-description", "{'invalid-arg':'aggregate-3'}", ValidationResultType.Critical);
            try
            {
                target.Initialize(_config);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Invalid args [{'invalid-arg':'aggregate-3'}]", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void Initialize_Given_an_empty_args_Should_throw_an_exception()
        {
            var target = new MatchesAggregateRule
            {
                Description = "rule-description",
                FailValidationResult = ValidationResultType.Critical,
                Name = "rule-name"
            };

            try
            {
                target.Initialize(_config);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Property TArgs cannot be empty or null", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void Args_Given_an_empty_args_Should_throw_an_exception()
        {
            try
            {
                CreateRule("rule-name", "rule-description", "", ValidationResultType.Warning);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Args is empty", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void Args_Given_an_invalid_json_args_Should_throw_an_exception()
        {
            try
            {
                CreateRule("rule-name", "rule-description", "{'ruleValue':'aggregate-3'|", ValidationResultType.Warning);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Error reading Args [{'ruleValue':'aggregate-3'|]", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        public MatchesAggregateRule CreateRule(string name, string description, string args, ValidationResultType failValidationResult)
        {
            return new MatchesAggregateRule
            {
                Description = description,
                FailValidationResult = failValidationResult,
                Name = name,
                Args = args
            };
        }
    }
}
