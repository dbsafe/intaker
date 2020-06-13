using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataProcessor.Rules.Tests
{
    [TestClass]
    public class MaxDateFieldRuleTest
    {
        private readonly FieldRuleConfiguration _config = new FieldRuleConfiguration();

        [TestMethod]
        public void Validate_Given_a_date_smaller_than_ruleValue_ValidationResult_should_be_valid()
        {
            var target = CreateRule("rule-name", "rule-description", "{'ruleValue':'2020-01-10'}", ValidationResultType.InvalidFixable);
            target.Initialize(_config);

            var field = new Field
            {
                Value = "2019-10-10",
                ValidationResult = ValidationResultType.Valid
            };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Validate_Given_a_date_equal_to_ruleValue_ValidationResult_should_be_valid()
        {
            var target = CreateRule("rule-name", "rule-description", "{'ruleValue':'2020-01-10'}", ValidationResultType.InvalidCritical);
            target.Initialize(_config);

            var field = new Field
            {
                Value = "2020-01-10",
                ValidationResult = ValidationResultType.Valid
            };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Validate_Given_a_date_greater_than_ruleValue_ValidationResult_should_be_set_with_the_value_from_the_rule()
        {
            var target = CreateRule("rule-name", "rule-description", "{'ruleValue':'2020-01-10'}", ValidationResultType.InvalidCritical);
            target.Initialize(_config);

            var field = new Field
            {
                Value = "2020-10-10",
                ValidationResult = ValidationResultType.Valid
            };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.InvalidCritical, field.ValidationResult);
        }

        [TestMethod]
        public void Initialize_Given_an_invalid_args_Should_throw_an_exception()
        {
            var target = CreateRule("rule-name", "rule-description", "{'invalid-arg':'2020-01-10'}", ValidationResultType.InvalidCritical);
            try
            {
                target.Initialize(_config);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Invalid args [{'invalid-arg':'2020-01-10'}]", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void Initialize_Given_an_empty_args_Should_throw_an_exception()
        {
            var target = new MaxDateFieldRule
            {
                Description = "rule-description",
                FailValidationResult = ValidationResultType.InvalidCritical,
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
                CreateRule("rule-name", "rule-description", "", ValidationResultType.InvalidFixable);
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
                CreateRule("rule-name", "rule-description", "{'ruleValue':'2020-01-10'|", ValidationResultType.InvalidFixable);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Error reading Args [{'ruleValue':'2020-01-10'|]", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void Args_Given_an_invalid_date_in_args_Should_throw_an_exception()
        {
            try
            {
                CreateRule("rule-name", "rule-description", "{'ruleValue':'2020-0a-10'", ValidationResultType.InvalidFixable);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Error reading Args [{'ruleValue':'2020-0a-10']", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        public MaxDateFieldRule CreateRule(string name, string description, string args, ValidationResultType? failValidationResult)
        {
            return new MaxDateFieldRule
            {
                Description = description,
                FailValidationResult = failValidationResult,
                Name = name,
                Args = args
            };
        }
    }
}
