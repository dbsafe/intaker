using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.ProcessorDefinition.Models;
using DataProcessor.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DataProcessor
{
    public static class ParsedDataProcessorHelper
    {
        public static ValidationResultType GetMaxValidationResult(ValidationResultType value1, ValidationResultType value2)
        {
            return value1.CompareTo(value2) > 0 ? value1 : value2;
        }

        public static void ValidateRowProcessorDefinition(string lineType, RowProcessorDefinition rowProcessorDefinition)
        {
            if (rowProcessorDefinition is null)
            {
                throw new ArgumentNullException($"{lineType} - {nameof(rowProcessorDefinition)}");
            }

            if (rowProcessorDefinition.FieldProcessorDefinitions is null)
            {
                throw new ArgumentNullException($"{lineType} - {nameof(rowProcessorDefinition.FieldProcessorDefinitions)}");
            }

            for (int i = 0; i < rowProcessorDefinition.FieldProcessorDefinitions.Length; i++)
            {
                var fieldProcessorDefinition = rowProcessorDefinition.FieldProcessorDefinitions[i];
                if (fieldProcessorDefinition.Decoder == null)
                {
                    throw new ArgumentNullException($"{lineType} - {nameof(fieldProcessorDefinition.Decoder)}");
                }
            }
        }

        public static void ValidateNumerOfFields(string lineType, Row row, RowProcessorDefinition rowProcessorDefinition)
        {
            if (rowProcessorDefinition is null)
            {
                throw new ArgumentNullException(nameof(rowProcessorDefinition));
            }

            if (rowProcessorDefinition.FieldProcessorDefinitions is null)
            {
                throw new ArgumentNullException(nameof(rowProcessorDefinition.FieldProcessorDefinitions));
            }

            if (rowProcessorDefinition.FieldProcessorDefinitions.Length != row.RawFields.Length)
            {
                row.ValidationResult = ValidationResultType.Error;
                var error = $"{lineType} - The expected number of fields {rowProcessorDefinition.FieldProcessorDefinitions.Length} is not equal to the actual number of fields {row.RawFields.Length}";
                row.Errors.Add(error);
            }
        }

        public static void SetJson(Row row, FieldProcessorDefinition[] fieldProcessorDefinitions)
        {
            var jProperties = new List<JProperty>();
            for (int i = 0; i < row.Fields.Count; i++)
            {
                var field = row.Fields[i];
                var fieldProcessorDefinition = fieldProcessorDefinitions[i];
                jProperties.Add(new JProperty(fieldProcessorDefinition.FieldName, field.Value));
            }

            var json = new JObject(jProperties);
            row.Json = json.ToString(Formatting.None);
        }

        public static void ProcessField(string description, Field field, FieldProcessorDefinition fieldProcessorDefinition)
        {
            try
            {
                if (field.ValidationResult != ValidationResultType.Valid)
                {
                    return;
                }

                DecodeField(fieldProcessorDefinition.FieldName, description, field, fieldProcessorDefinition.Decoder);

                if (field.ValidationResult != ValidationResultType.Valid)
                {
                    return;
                }

                ValidateFieldRules(field, fieldProcessorDefinition.Rules);

                if (field.ValidationResult != ValidationResultType.Valid)
                {
                    return;
                }

                ProcessAggregators(field, fieldProcessorDefinition.Aggregators);
            }
            catch (Exception ex)
            {
                throw new ParsedDataProcessorException($"RowIndex: {field.Row.Index}, FieldIndex: {field.Index}, Field: {description}", ex);
            }
        }

        private static void DecodeField(string fieldName, string description, Field field, IFieldDecoder fieldDecoder)
        {
            DataProcessorGlobal.Debug($"Decoding Field: {fieldName}, Raw Value: '{field.Raw}'");
            fieldDecoder.Decode(field);

            if (field.ValidationResult == ValidationResultType.Valid)
            {
                return;
            }

            field.Row.ValidationResult = ParsedDataProcessorHelper.GetMaxValidationResult(field.Row.ValidationResult, field.ValidationResult);
            var message = $"Invalid {description} '{field.Raw}'";

            if (field.ValidationResult == ValidationResultType.Warning)
            {
                field.Row.Warnings.Add(message);
            }
            else
            {
                field.Row.Errors.Add(message);
            }
        }

        private static void ValidateFieldRules(Field field, IFieldRule[] fieldRules)
        {
            if (fieldRules == null || fieldRules.Length == 0)
            {
                return;
            }

            var tempValidationResultType = field.ValidationResult;
            foreach (var fieldRule in fieldRules)
            {
                ProcessRule(field, fieldRule);
                tempValidationResultType = ParsedDataProcessorHelper.GetMaxValidationResult(tempValidationResultType, field.ValidationResult);
            }

            field.ValidationResult = tempValidationResultType;
            field.Row.ValidationResult = ParsedDataProcessorHelper.GetMaxValidationResult(field.Row.ValidationResult, field.ValidationResult);
        }

        private static void ProcessAggregators(Field field, IFieldAggregator[] aggregators)
        {
            if (aggregators == null || aggregators.Length == 0)
            {
                return;
            }

            foreach (var aggregator in aggregators)
            {
                DataProcessorGlobal.Debug($"Processing Aggregator: {aggregator.Name}, Value: {aggregator.Aggregate.Value}");
                aggregator.AggregateField(field);
                DataProcessorGlobal.Debug($"Processed Aggregator: {aggregator.Name}, New Value: {aggregator.Aggregate.Value}");
            }
        }

        private static void ProcessRule(Field field, IFieldRule fieldRule)
        {
            DataProcessorGlobal.Debug($"Processing Field Rule: {fieldRule.Name}");
            field.ValidationResult = ValidationResultType.Valid;
            fieldRule.Validate(field);

            if (field.ValidationResult == ValidationResultType.Valid)
            {
                return;
            }

            DataProcessorGlobal.Debug($"Field Rule {fieldRule.Name} failed");
            if (field.ValidationResult == ValidationResultType.Warning)
            {
                field.Row.Warnings.Add(fieldRule.Description);
            }
            else
            {
                field.Row.Errors.Add(fieldRule.Description);
            }
        }
    }
}
