using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.ProcessorDefinition.Models;
using DataProcessor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor
{
    public class ParsedDataProcessor20
    {
        private readonly IDataSource<ParserContext20> _source;
        private readonly bool _hasHeader;
        private readonly bool _hasTrailer;

        private readonly FileProcessorDefinition20 _fileProcessorDefinition;

        public ParserContext20 ParserContext { get; private set; }

        public ParsedDataProcessor20(IDataSource<ParserContext20> source, FileProcessorDefinition20 fileProcessorDefinition)
        {
            _source = source;
            _hasHeader = fileProcessorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions.Length > 0;
            _hasTrailer = fileProcessorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions.Length > 0;

            ValidateProcessorDefinition(fileProcessorDefinition);

            _fileProcessorDefinition = fileProcessorDefinition;

            _source.BeforeProcessRow += SourceBeforeProcessRow;
            _source.AfterProcessRow += SourceAfterProcessRow;
            _source.ProcessField += SourceProcessField;
        }

        private static void ValidateProcessorDefinition(FileProcessorDefinition20 processorDefinition)
        {
            if (processorDefinition is null)
            {
                throw new ArgumentNullException(nameof(processorDefinition));
            }

            ParsedDataProcessorHelper.ValidateRowProcessorDefinition("Header", processorDefinition.HeaderRowProcessorDefinition);
            ParsedDataProcessorHelper.ValidateRowProcessorDefinition("Trailer", processorDefinition.TrailerRowProcessorDefinition);
            ValidateDataRowProcessorDefinitions(processorDefinition);
        }

        private void SourceBeforeProcessRow(object sender, ProcessRowEventArgs<ParserContext20> e)
        {
            DataProcessorGlobal.Debug($"Processing Row. Index: {e.Row.Index}, Raw Data: '{e.Row.Raw}'");
            e.Context.DataType = string.Empty;

            string lineType;
            RowProcessorDefinition rowProcessorDefinition;

            if (IsHeaderRow(e.Row))
            {
                lineType = "Header Row";
                rowProcessorDefinition = _fileProcessorDefinition.HeaderRowProcessorDefinition;
                e.Context.Header = e.Row;
                ParsedDataProcessorHelper.ValidateNumerOfFields(lineType, e.Row, rowProcessorDefinition);
                return;
            }

            if (IsTrailerRow(e.Context.IsCurrentRowTheLast))
            {
                lineType = "Trailer Row";
                rowProcessorDefinition = _fileProcessorDefinition.TrailerRowProcessorDefinition;
                e.Context.Trailer = e.Row;
                ParsedDataProcessorHelper.ValidateNumerOfFields(lineType, e.Row, rowProcessorDefinition);
                return;
            }

            ValidateNumerOfFields(_fileProcessorDefinition, e);
        }

        private bool IsHeaderRow(Row row)
        {
            return row.Index == 0 && _hasHeader;
        }

        private bool IsTrailerRow(bool isCurrentRowTheLast)
        {
            return isCurrentRowTheLast && _hasTrailer;
        }

        private static void ValidateNumerOfFields(FileProcessorDefinition20 fileProcessorDefinition, ProcessRowEventArgs<ParserContext20> e)
        {
            var kvp = FindDataRowProcessorDefinition(fileProcessorDefinition, e);
            if (kvp.Key == null)
            {
                e.Row.ValidationResult = ValidationResultType.Error;
                var error = $"Unknown line type";
                e.Row.Errors.Add(error);
                return;
            }

            e.Context.DataRows.Add(e.Row);
            e.Context.DataType = kvp.Key;
            ParsedDataProcessorHelper.ValidateNumerOfFields(kvp.Key, e.Row, kvp.Value.RowProcessorDefinition);
        }

        private static KeyValuePair<string, DataRowProcessorDefinition> FindDataRowProcessorDefinition(FileProcessorDefinition20 fileProcessorDefinition, ProcessRowEventArgs<ParserContext20> e)
        {
            foreach (var kvp in fileProcessorDefinition.DataRowProcessorDefinitions)
            {
                if (kvp.Value.DataTypeFieldIndex >= e.Row.RawFields.Length)
                {
                    continue;
                }

                if (e.Row.RawFields[kvp.Value.DataTypeFieldIndex] == kvp.Key)
                {
                    return kvp;
                }
            }

            return default;
        }

        private void SourceAfterProcessRow(object sender, ProcessRowEventArgs<ParserContext20> e)
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
                    ParsedDataProcessorHelper.SetJson(e.Row, _fileProcessorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions);
                    return;
                }

                if (IsTrailerRow(e.Context.IsCurrentRowTheLast))
                {
                    ParsedDataProcessorHelper.SetJson(e.Row, _fileProcessorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions);
                    return;
                }

                var dataRowProcessorDefinition = _fileProcessorDefinition.DataRowProcessorDefinitions[e.Context.DataType];
                ParsedDataProcessorHelper.SetJson(e.Row, dataRowProcessorDefinition.RowProcessorDefinition.FieldProcessorDefinitions);
            }
        }

        private void SourceProcessField(object sender, ProcessFieldEventArgs<ParserContext20> e)
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
                var dataRowProcessorDefinition = _fileProcessorDefinition.DataRowProcessorDefinitions[e.Context.DataType];
                fieldProcessorDefinition = dataRowProcessorDefinition.RowProcessorDefinition.FieldProcessorDefinitions[e.Field.Index];
            }

            ParsedDataProcessorHelper.ProcessField(fieldProcessorDefinition.Description, e.Field, fieldProcessorDefinition);
        }

        private static void ValidateDataRowProcessorDefinitions(FileProcessorDefinition20 processorDefinition)
        {
            if (processorDefinition.DataRowProcessorDefinitions is null)
            {
                throw new ArgumentNullException($"{nameof(processorDefinition.DataRowProcessorDefinitions)}");
            }

            ValidateKeyField(processorDefinition);

            foreach (var item in processorDefinition.DataRowProcessorDefinitions.ToArray())
            {
                ParsedDataProcessorHelper.ValidateRowProcessorDefinition(item.Key, item.Value.RowProcessorDefinition);
            }
        }

        private static void ValidateKeyField(FileProcessorDefinition20 processorDefinition)
        {
            if (string.IsNullOrWhiteSpace(processorDefinition.KeyField))
            {
                throw new ArgumentNullException($"{nameof(processorDefinition.KeyField)} cannot be null or empty");
            }

            foreach (var value in processorDefinition.DataRowProcessorDefinitions.Values)
            {
                var keyFieldFound = value.RowProcessorDefinition.FieldProcessorDefinitions.Any(a => a.FieldName == processorDefinition.KeyField);
                if (!keyFieldFound)
                {
                    throw new ArgumentNullException($"{nameof(processorDefinition.KeyField)} - A field with name '{processorDefinition.KeyField}' was not found in every data element");
                }
            }
        }

        public ParsedData Process()
        {
            ParserContext = new ParserContext20 { ValidationResult = ValidationResultType.Valid };
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
