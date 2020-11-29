using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.Utils;
using DataProcessor.ProcessorDefinition.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DataProcessor
{
    public class ParsedDataProcessor
    {
        private readonly IDataSource _source;
        private readonly FileProcessorDefinition _fileProcessorDefinition;
        private readonly bool _hasHeader;
        private readonly bool _hasTrailer;

        public ParserContext ParserContext { get; private set; }

        public ParsedDataProcessor(IDataSource source, FileProcessorDefinition fileProcessorDefinition)
        {
            ValidateProcessorDefinition(fileProcessorDefinition);

            _hasHeader = fileProcessorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions.Length > 0;
            _hasTrailer = fileProcessorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions.Length > 0;

            _source = source;
            _fileProcessorDefinition = fileProcessorDefinition;

            _source.BeforeProcessRow += SourceBeforeProcessRow;
            _source.AfterProcessRow += SourceAfterProcessRow;
            _source.ProcessField += SourceProcessField;
        }

        private static void ValidateProcessorDefinition(FileProcessorDefinition processorDefinition)
        {
            if (processorDefinition is null)
            {
                throw new ArgumentNullException(nameof(processorDefinition));
            }

            ValidateRowProcessorDefinition("Header", processorDefinition.HeaderRowProcessorDefinition);
            ValidateRowProcessorDefinition("Data", processorDefinition.DataRowProcessorDefinition);
            ValidateRowProcessorDefinition("Trailer", processorDefinition.TrailerRowProcessorDefinition);
        }

        private static void ValidateRowProcessorDefinition(string lineType, RowProcessorDefinition rowProcessorDefinition)
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

        private bool IsHeaderRow(Row row)
        {
            return row.Index == 0 && _hasHeader;
        }

        private bool IsTrailerRow(bool isCurrentRowTheLast)
        {
            return isCurrentRowTheLast && _hasTrailer;
        }

        private void SourceBeforeProcessRow(object sender, ProcessRowEventArgs e)
        {
            DataProcessorGlobal.Debug($"Processing Row. Index: {e.Row.Index}, Raw Data: '{e.Row.Raw}'");

            string lineType;
            RowProcessorDefinition rowProcessorDefinition;

            if (IsHeaderRow(e.Row))
            {
                lineType = "Header Row";
                rowProcessorDefinition = _fileProcessorDefinition.HeaderRowProcessorDefinition;
                e.Context.Header = e.Row;
                ValidateNumerOfFields(lineType, e.Row, rowProcessorDefinition);
                return;
            }

            if (IsTrailerRow(e.Context.IsCurrentRowTheLast))
            {
                lineType = "Trailer Row";
                rowProcessorDefinition = _fileProcessorDefinition.TrailerRowProcessorDefinition;
                e.Context.Trailer = e.Row;
                ValidateNumerOfFields(lineType, e.Row, rowProcessorDefinition);
                return;
            }

            lineType = "Data Row";
            rowProcessorDefinition = _fileProcessorDefinition.DataRowProcessorDefinition;
            e.Context.DataRows.Add(e.Row);
            ValidateNumerOfFields(lineType, e.Row, rowProcessorDefinition);
        }

        private void SourceAfterProcessRow(object sender, ProcessRowEventArgs e)
        {
            e.Context.AllRows.Add(e.Row);
            e.Context.ValidationResult = ParsedDataProcessorHelper.GetMaxValidationResult(e.Context.ValidationResult, e.Row.ValidationResult);
            if (e.Context.ValidationResult == ValidationResultType.Critical)
            {
                e.Context.IsAborted = true;
            }

            if (e.Row.ValidationResult != ValidationResultType.Valid && e.Row.ValidationResult != ValidationResultType.Warning)
            {
                e.Context.InvalidRows.Add(e.Row);

                if (IsHeaderRow(e.Row))
                {
                    e.Context.Errors.Add("Header row is invalid");
                    return;
                }

                if (IsTrailerRow(e.Context.IsCurrentRowTheLast))
                {
                    e.Context.Errors.Add("Trailer row is invalid");
                    return;
                }

                e.Context.InvalidDataRowCount++;
                return;
            }

            if (_fileProcessorDefinition.CreateRowJsonEnabled)
            {
                if (IsHeaderRow(e.Row))
                {
                    SetJson(e.Row, _fileProcessorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions);
                    return;
                }

                if (IsTrailerRow(e.Context.IsCurrentRowTheLast))
                {
                    SetJson(e.Row, _fileProcessorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions);
                    return;
                }

                SetJson(e.Row, _fileProcessorDefinition.DataRowProcessorDefinition.FieldProcessorDefinitions);
            }
        }

        private void SetJson(Row row, FieldProcessorDefinition[] fieldProcessorDefinitions)
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

        private void SourceProcessField(object sender, ProcessFieldEventArgs e)
        {
            if (e.Field.Row.ValidationResult == ValidationResultType.Critical)
            {
                return;
            }

            FieldProcessorDefinition fieldProcessorDefinition;

            if (e.Field.Row.Index == 0 && _hasHeader)
            {
                fieldProcessorDefinition = _fileProcessorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions[e.Field.Index];
            }
            else if (e.Context.IsCurrentRowTheLast && _hasTrailer)
            {
                fieldProcessorDefinition = _fileProcessorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions[e.Field.Index];
            }
            else
            {
                fieldProcessorDefinition = _fileProcessorDefinition.DataRowProcessorDefinition.FieldProcessorDefinitions[e.Field.Index];
            }

            ProcessField(fieldProcessorDefinition.Description, e.Field, fieldProcessorDefinition);
        }

        private void ProcessField(string description, Field field, FieldProcessorDefinition fieldProcessorDefinition)
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

        private void ProcessAggregators(Field field, IFieldAggregator[] aggregators)
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

        private void ValidateFieldRules(Field field, IFieldRule[] fieldRules)
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

        private void ProcessRule(Field field, IFieldRule fieldRule)
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

        private void DecodeField(string fieldName, string description, Field field, IFieldDecoder fieldDecoder)
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

        private void ValidateNumerOfFields(string lineType, Row row, RowProcessorDefinition rowProcessorDefinition)
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

        public ParsedData Process()
        {
            ParserContext = new ParserContext { ValidationResult = ValidationResultType.Valid };
            _source.Process(ParserContext);

            if (ParserContext.InvalidDataRowCount > 0)
            {
                if (ParserContext.InvalidDataRowCount == 1)
                {
                    ParserContext.Errors.Add($"There is 1 invalid data row");
                }
                else
                {
                    ParserContext.Errors.Add($"There are {ParserContext.InvalidDataRowCount} invalid data rows");
                }
            }

            return new ParsedData
            {
                Errors = ParserContext.Errors,
                AllRows = ParserContext.AllRows,
                DataRows = ParserContext.DataRows,
                InvalidRows = ParserContext.InvalidRows,
                Header = ParserContext.Header,
                Trailer = ParserContext.Trailer,
                ValidationResult = ParserContext.ValidationResult
            };
        }
    }
}
