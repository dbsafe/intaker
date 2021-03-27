using DataProcessor.Contracts;
using DataProcessor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
        public void Initialize_Given_missing_datetime_arg_Should_throw_an_exception()
        {
            try
            {
                var target = CreateRule("rule-name", "rule-description", "2020-01-aa", ValidationResultType.Critical);
                target.Initialize(_config);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Rule: 'rule-name'. Argument: 'DateTimeValue'. Invalid value '2020-01-aa'", ex.Message);
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
                Assert.AreEqual("Rule: 'rule-name'. Argument 'DateTimeValue' not found", ex.Message);
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
                Args = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("DateTimeValue", arg)
                }
            };
        }

        public MaxDateFieldRule CreateRuleWithoutArgs(string name, string description, ValidationResultType failValidationResult)
        {
            return new MaxDateFieldRule
            {
                Description = description,
                FailValidationResult = failValidationResult,
                Name = name
            };
        }
    }
}
