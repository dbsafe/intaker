using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.ProcessorDefinition.Models;
using DataProcessor.Transformations;
using DataProcessor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor
{
    public class ParsedDataProcessor20 : ParsedDataProcessor<ParserContext20>
    {
        private readonly FileProcessorDefinition20 _fileProcessorDefinition;

        public ParserContext20 ParserContext { get; private set; }

        public ParsedDataProcessor20(IDataSource<ParserContext20> source, FileProcessorDefinition20 fileProcessorDefinition)
            : base(source,
                   fileProcessorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions.Length > 0,
                   fileProcessorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions.Length > 0)
        {
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

        private void ClearContext(ParserContext20 context)
        {
            context.DataType = string.Empty;
            context.CurrentDataRow20 = null;
            context.DataTypeFieldIndex = -1;
            context.DataKeyFieldIndex = -1;
        }

        private void SourceBeforeProcessRow(object sender, ProcessRowEventArgs<ParserContext20> e)
        {
            DataProcessorGlobal.Debug($"Processing Row. Index: {e.Row.Index}, Raw Data: '{e.Row.Raw}'");
            ClearContext(e.Context);

            if (IsHeaderRow(e.Row))
            {
                BeforeProcessHeaderRow(e);
                return;
            }

            if (IsTrailerRow(e.Context.IsCurrentRowTheLast))
            {
                BeforeProcessTrailerRow(e);
                return;
            }

            SourceBeforeProcessDataRow(e);
        }

        private void SourceBeforeProcessDataRow(ProcessRowEventArgs<ParserContext20> e)
        {
            e.Context.CurrentDataRow20 = new DataRow20 { Row = e.Row };
            
            var kvp = FindDataRowProcessorDefinition(_fileProcessorDefinition, e);
            if (kvp.Key == null)
            {
                e.Row.ValidationResult = ValidationResultType.Error;
                var error = $"Unknown line type";
                e.Row.Errors.Add(error);
                e.Context.DataRowsWithInvalidTypes.Add(e.Context.CurrentDataRow20);
                return;
            }

            e.Context.DataRows.Add(e.Context.CurrentDataRow20);
            e.Context.DataType = kvp.Key;
            e.Context.DataTypeFieldIndex = kvp.Value.DataTypeFieldIndex;
            e.Context.DataKeyFieldIndex = kvp.Value.DataKeyFieldIndex;
            var lineType = $"Data Row '{kvp.Key}'";
            ParsedDataProcessorHelper.ValidateNumerOfFields(lineType, e.Row, kvp.Value.RowProcessorDefinition);
        }

        private void BeforeProcessHeaderRow(ProcessRowEventArgs<ParserContext20> e)
        {
            e.Context.Header = e.Row;
            ParsedDataProcessorHelper.ValidateNumerOfFields("Header Row", e.Row, _fileProcessorDefinition.HeaderRowProcessorDefinition);
        }

        private void BeforeProcessTrailerRow(ProcessRowEventArgs<ParserContext20> e)
        {
            e.Context.Trailer = e.Row;
            ParsedDataProcessorHelper.ValidateNumerOfFields("Trailer Row", e.Row, _fileProcessorDefinition.TrailerRowProcessorDefinition);
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
            e.Context.ValidationResult = ParsedDataProcessorHelper.GetMaxValidationResult(e.Context.ValidationResult, e.Row.ValidationResult);
            if (e.Context.ValidationResult == ValidationResultType.Critical)
            {
                e.Context.IsAborted = true;
            }

            if (IsHeaderRow(e.Row))
            {
                AfterProcessHeaderRow(e);
                return;
            }

            if (IsTrailerRow(e.Context.IsCurrentRowTheLast))
            {
                AfterProcessTrailerRow(e);
                return;
            }

            AfterProcessDataRow(e);
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

        private void AfterProcessHeaderRow(ProcessRowEventArgs<ParserContext20> e)
        {
            if (!IsValidRow(e))
            {
                e.Context.Errors.Add("Header row is invalid");
                return;
            }

            if (_fileProcessorDefinition.CreateRowJsonEnabled)
            {
                ParsedDataProcessorHelper.SetJson(e.Row, _fileProcessorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions);
            }
        }

        private void AfterProcessTrailerRow(ProcessRowEventArgs<ParserContext20> e)
        {
            if (!IsValidRow(e))
            {
                e.Context.Errors.Add("Trailer row is invalid");
                return;
            }

            if (_fileProcessorDefinition.CreateRowJsonEnabled)
            {
                ParsedDataProcessorHelper.SetJson(e.Row, _fileProcessorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions);
            }
        }

        private void AfterProcessDataRow(ProcessRowEventArgs<ParserContext20> e)
        {
            if (e.Context.CurrentDataRow20 == null)
            {
                throw new InvalidOperationException("CurrentDataRow20 is null");
            }

            SetDataTypeAndDataKey(e.Context);

            if (!IsValidRow(e))
            {
                e.Context.InvalidDataRows.Add(e.Context.CurrentDataRow20);
                return;
            }

            if (_fileProcessorDefinition.CreateRowJsonEnabled)
            {
                var dataRowProcessorDefinition = _fileProcessorDefinition.DataRowProcessorDefinitions[e.Context.DataType];
                ParsedDataProcessorHelper.SetJson(e.Row, dataRowProcessorDefinition.RowProcessorDefinition.FieldProcessorDefinitions);
            }
        }

        private static void SetDataTypeAndDataKey(ParserContext20 context)
        {
            var fieldCount = context.CurrentDataRow20.Row.Fields.Count;
            if (context.DataTypeFieldIndex > -1 && context.DataTypeFieldIndex < fieldCount)
            {
                context.CurrentDataRow20.DataType = context.CurrentDataRow20.Row.Fields[context.DataTypeFieldIndex].Value.ToString();
            }

            if (context.DataKeyFieldIndex > -1 && context.DataKeyFieldIndex < fieldCount)
            {
                context.CurrentDataRow20.DataKey = context.CurrentDataRow20.Row.Fields[context.DataKeyFieldIndex].Value.ToString();
            }
        }

        public ParsedData20 Process()
        {
            ParserContext = new ParserContext20 { ValidationResult = ValidationResultType.Valid };
            _source.Process(ParserContext);
            VerifyInvalidDataRows(ParserContext);

            return new ParsedData20
            {
                Errors = ParserContext.Errors,
                DataRows = ParserContext.DataRows,
                InvalidDataRows = ParserContext.InvalidDataRows,
                Header = ParserContext.Header,
                Trailer = ParserContext.Trailer,
                ValidationResult = ParserContext.ValidationResult,
                DataRowsWithInvalidTypes = ParserContext.DataRowsWithInvalidTypes
            };
        }
    }
}
