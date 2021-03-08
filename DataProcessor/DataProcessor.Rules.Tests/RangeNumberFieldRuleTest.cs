using DataProcessor.Contracts;
using DataProcessor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DataProcessor.Rules.Tests
{
    [TestClass]
    public class RangeNumberFieldRuleTest
    {
        private readonly FieldRuleConfiguration _config = new FieldRuleConfiguration();

        private readonly KeyValuePair<string, string>[] _validArgs = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("Min", "10"),
            new KeyValuePair<string, string>("Max", "100")
        };

        private readonly KeyValuePair<string, string>[] _invalidArgs = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("Min", "10"),
            new KeyValuePair<string, string>("Max", "1aa")
        };

        private readonly KeyValuePair<string, string>[] _incompleArgs = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("Min", "10"),
        };

        [TestMethod]
        public void Validate_Given_a_number_smaller_than_min_ValidationResult_should_be_set_with_the_value_from_the_rule()
        {
            var target = CreateRule("rule-name", "rule-description", _validArgs, ValidationResultType.Critical);
            target.Initialize(_config);

            var field = new Field
            {
                Value = "2",
                ValidationResult = ValidationResultType.Valid
            };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Critical, field.ValidationResult);
        }

        [TestMethod]
        public void Validate_Given_a_number_in_the_range_ValidationResult_should_be_valid()
        {
            var target = CreateRule("rule-name", "rule-description", _validArgs, ValidationResultType.Critical);
            target.Initialize(_config);

            var field = new Field
            {
                Value = "20",
                ValidationResult = ValidationResultType.Valid
            };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Valid, field.ValidationResult);
        }

        [TestMethod]
        public void Validate_Given_a_number_greater_than_max_ValidationResult_should_be_set_with_the_value_from_the_rule()
        {
            var target = CreateRule("rule-name", "rule-description", _validArgs, ValidationResultType.Error);
            target.Initialize(_config);

            var field = new Field
            {
                Value = "200",
                ValidationResult = ValidationResultType.Valid
            };

            target.Validate(field);

            Assert.AreEqual(ValidationResultType.Error, field.ValidationResult);
        }

        [TestMethod]
        public void Args_Given_an_invalid_arg_Should_throw_an_exception()
        {
            try
            {
                var target = CreateRule("rule-name", "rule-description", _invalidArgs, ValidationResultType.Error);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Invalid arg 'Max', found '1aa'", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void Args_Given_a_null_arg_Should_throw_an_exception()
        {
            try
            {
                var target = CreateRule("rule-name", "rule-description", null, ValidationResultType.Error);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Args cannot be null or empty", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        [TestMethod]
        public void Args_Given_an_arg_with_missing_values_Should_throw_an_exception()
        {
            try
            {
                var target = CreateRule("rule-name", "rule-description", _incompleArgs, ValidationResultType.Error);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("RuleName: rule-name, RuleDescription: rule-description - Arg 'Max' not found", ex.Message);
                return;
            }

            Assert.Fail($"An {nameof(InvalidOperationException)} was not thrown");
        }

        public RangeNumberFieldRule CreateRule(string name, string description, KeyValuePair<string, string>[] args, ValidationResultType failValidationResult)
        {
            return new RangeNumberFieldRule
            {
                Description = description,
                FailValidationResult = failValidationResult,
                Name = name,
                Args = args
            };
        }
    }
}
