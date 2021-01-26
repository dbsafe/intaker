using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.Utils;
using DataProcessor.ProcessorDefinition.Models;
using System;

namespace DataProcessor
{
    public class ParsedDataProcessor10 : ParsedDataProcessor
    {
        private readonly FileProcessorDefinition10 _fileProcessorDefinition;

        public ParserContext ParserContext { get; private set; }

        public ParsedDataProcessor10(IDataSource source, FileProcessorDefinition10 fileProcessorDefinition)
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

        private static void ValidateProcessorDefinition(FileProcessorDefinition10 processorDefinition)
        {
            if (processorDefinition is null)
            {
                throw new ArgumentNullException(nameof(processorDefinition));
            }

            ValidateRowProcessorDefinition("Header", processorDefinition.HeaderRowProcessorDefinition);
            ValidateRowProcessorDefinition("Data", processorDefinition.DataRowProcessorDefinition);
            ValidateRowProcessorDefinition("Trailer", processorDefinition.TrailerRowProcessorDefinition);
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
