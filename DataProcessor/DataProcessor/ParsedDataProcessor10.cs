using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.Utils;
using DataProcessor.ProcessorDefinition.Models;
using System;

namespace DataProcessor
{
    public class ParsedDataProcessor10 : ParsedDataProcessor<ParserContext10>
    {
        private readonly FileProcessorDefinition10 _fileProcessorDefinition;

        public ParserContext10 ParserContext { get; private set; }

        public ParsedDataProcessor10(IDataSource<ParserContext10> source, FileProcessorDefinition10 fileProcessorDefinition)
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

            ParsedDataProcessorHelper.ValidateRowProcessorDefinition("Header", processorDefinition.HeaderRowProcessorDefinition);
            ParsedDataProcessorHelper.ValidateRowProcessorDefinition("Data", processorDefinition.DataRowProcessorDefinition);
            ParsedDataProcessorHelper.ValidateRowProcessorDefinition("Trailer", processorDefinition.TrailerRowProcessorDefinition);
        }

        private void SourceBeforeProcessRow(object sender, ProcessRowEventArgs<ParserContext10> e)
        {
            DataProcessorGlobal.Debug($"Processing Row. Index: {e.Row.Index}, Raw Data: '{e.Row.Raw}'");

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

            lineType = "Data Row";
            rowProcessorDefinition = _fileProcessorDefinition.DataRowProcessorDefinition;
            e.Context.DataRows.Add(e.Row);
            ParsedDataProcessorHelper.ValidateNumerOfFields(lineType, e.Row, rowProcessorDefinition);
        }

        private void SourceAfterProcessRow(object sender, ProcessRowEventArgs<ParserContext10> e)
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

                ParsedDataProcessorHelper.SetJson(e.Row, _fileProcessorDefinition.DataRowProcessorDefinition.FieldProcessorDefinitions);
            }
        }

        private void SourceProcessField(object sender, ProcessFieldEventArgs<ParserContext10> e)
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

            ParsedDataProcessorHelper.ProcessField(fieldProcessorDefinition.Description, e.Field, fieldProcessorDefinition);
        }

        public ParsedData10 Process()
        {
            ParserContext = new ParserContext10 { ValidationResult = ValidationResultType.Valid };
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

            return new ParsedData10
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
