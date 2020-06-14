using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataProcessor.Rules.Tests
{
    [TestClass]
    public class MaxNumberFieldRuleTest
    {
        private readonly FieldRuleConfiguration _config = new FieldRuleConfiguration();

        [TestMethod]
        public void Validate_Given_a_number_greater_than_ruleValue_ValidationResult_should_be_set_with_the_value_from_the_rule()
        {
            var target = CreateRule("rule-name", "rule-description", "{'ruleValue':'10'}", ValidationResultType.InvalidFixable);
            target.Initialize(_config);

            var field = new Field
            {
                Value = "100",
                ValidationResult = ValidationResultType.Valid
            };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.InvalidFixable, field.ValidationResult);
        }

        [TestMethod]
        public void Validate_Given_a_number_equal_to_ruleValue_ValidationResult_should_be_valid()
        {
            var target = CreateRule("rule-name", "rule-description", "{'ruleValue':'10'}", ValidationResultType.InvalidCritical);
            target.Initialize(_config);

            var field = new Field
            {
                Value = "10",
                ValidationResult = ValidationResultType.Valid
            };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Validate_Given_a_number_smaller_than_ruleValue_ValidationResult_should_be_valid()
        {
            var target = CreateRule("rule-name", "rule-description", "{'ruleValue':'10'}", ValidationResultType.InvalidCritical);
            target.Initialize(_config);

            var field = new Field
            {
                Value = "1",
                ValidationResult = ValidationResultType.Valid
            };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Initialize_Given_an_invalid_args_Should_throw_an_exception()
        {
            var target = CreateRule("rule-name", "rule-description", "{'invalid-arg':'10'}", ValidationResultType.InvalidCritical);
            try
            {
                target.Initialize(_config);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Invalid args [{'invalid-arg':'10'}]", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        public MaxNumberFieldRule CreateRule(string name, string description, string args, ValidationResultType failValidationResult)
        {
            return new MaxNumberFieldRule
            {
                Description = description,
                FailValidationResult = failValidationResult,
                Name = name,
                Args = args
            };
        }
    }
}
