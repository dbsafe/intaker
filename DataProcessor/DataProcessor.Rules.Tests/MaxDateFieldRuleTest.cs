using DataProcessor.Contracts;
using DataProcessor.Models;
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
            var target = CreateRule("rule-name", "rule-description", "2020-01-10", ValidationResultType.Warning);
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
            var target = CreateRule("rule-name", "rule-description", "2020-01-10", ValidationResultType.Critical);
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
            var target = CreateRule("rule-name", "rule-description", "2020-01-10", ValidationResultType.Critical);
            target.Initialize(_config);

            var field = new Field
            {
                Value = "2020-10-10",
                ValidationResult = ValidationResultType.Valid
            };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Critical, field.ValidationResult);
        }

        [TestMethod]
        public void SingleArg_Given_an_invalid_arg_Should_throw_an_exception()
        {
            try
            {
                CreateRule("rule-name", "rule-description", "2020-01-aa", ValidationResultType.Critical);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Invalid arg '2020-01-aa'", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void SingleArg_Given_a_null_arg_Should_throw_an_exception()
        {
            try
            {
                CreateRule("rule-name", "rule-description", null, ValidationResultType.Critical);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Invalid arg ''", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        public MaxDateFieldRule CreateRule(string name, string description, string arg, ValidationResultType failValidationResult)
        {
            return new MaxDateFieldRule
            {
                Description = description,
                FailValidationResult = failValidationResult,
                Name = name,
                SingleArg = arg
            };
        }
    }
}
