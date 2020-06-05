using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
using DataProcessor.InputDefinitionFile;
using DataProcessor.ProcessorDefinition.Models;
using System;

namespace DataProcessor
{
    public class ParsedDataProcessor
    {
        private readonly IDataSource _source;
        private readonly ProcessorDefinition.Models.ProcessorDefinition _processorDefinition;
        private bool _hasHeader;
        private bool _hasTrailer;

        public ParserContext ParserContext { get; private set; }

        public ParsedDataProcessor(IDataSource source, ProcessorDefinition.Models.ProcessorDefinition processorDefinition)
        {
            ValidateProcessorDefinition(processorDefinition);

            _hasHeader = processorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions.Length > 0;
            _hasTrailer = processorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions.Length > 0;

            _source = source;
            _processorDefinition = processorDefinition;

            _source.BeforeProcessRow += SourceBeforeProcessRow;
            _source.AfterProcessRow += SourceAfterProcessRow;
            _source.ProcessField += SourceProcessField;
        }

        public ParsedDataProcessor(IDataSource source, InputDefinitionFile_10 inputDefinitionFile_10)
        {
        }

        private static void ValidateProcessorDefinition(ProcessorDefinition.Models.ProcessorDefinition processorDefinition)
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

        private void SourceBeforeProcessRow(object sender, ProcessRowEventArgs e)
        {
            string lineType;
            RowProcessorDefinition rowProcessorDefinition;

            if (e.Row.Index == 0 && _hasHeader)
            {
                lineType = "Header Line";
                rowProcessorDefinition = _processorDefinition.HeaderRowProcessorDefinition;
                e.Context.Header = e.Row;
            }
            else if (e.Context.IsCurrentRowTheLast && _hasTrailer)
            {
                lineType = "Trailer Line";
                rowProcessorDefinition = _processorDefinition.TrailerRowProcessorDefinition;
                e.Context.Trailer = e.Row;
            }
            else
            {
                lineType = "Data Line";
                rowProcessorDefinition = _processorDefinition.DataRowProcessorDefinition;
            }

            ValidateNumerOfFields(lineType, e.Row, rowProcessorDefinition);
        }

        private void SourceAfterProcessRow(object sender, ProcessRowEventArgs e)
        {
            e.Context.Rows.Add(e.Row);

            if (e.Row.ValidationResult != ValidationResultType.Valid)
            {
                e.Context.InvalidRows.Add(e.Row);
            }

            if (e.Row.ValidationResult == ValidationResultType.InvalidCritical)
            {
                return;
            }
        }

        private void SourceProcessField(object sender, ProcessFieldEventArgs e)
        {
            if (e.Field.Row.ValidationResult == ValidationResultType.InvalidCritical)
            {
                return;
            }

            FieldProcessorDefinition fieldProcessorDefinition;

            if (e.Field.Row.Index == 0 && _hasHeader)
            {
                fieldProcessorDefinition = _processorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions[e.Field.Index];
            }
            else if (e.Context.IsCurrentRowTheLast && _hasTrailer)
            {
                fieldProcessorDefinition = _processorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions[e.Field.Index];
            }
            else
            {
                fieldProcessorDefinition = _processorDefinition.DataRowProcessorDefinition.FieldProcessorDefinitions[e.Field.Index];
            }

            ProcessField(fieldProcessorDefinition.FieldName, e.Field, fieldProcessorDefinition.Decoder);
        }

        private void ProcessField(string fieldName, Field field, IFieldDecoder fieldDecoder)
        {
            try
            {
                fieldDecoder.Decode(field);
            }
            catch (Exception ex)
            {
                throw new ParsedDataProcessorException($"RowIndex: {field.Row.Index}, FieldIndex: {field.Index}, FieldName: {fieldName}", ex);
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
                row.ValidationResult = ValidationResultType.InvalidCritical;
                var error = $"{lineType} - The expected number of fields {rowProcessorDefinition.FieldProcessorDefinitions.Length} is not equal to the actual number of fields {row.RawFields.Length}";
                row.Errors.Add(error);
            }
        }

        public ParsedData Process()
        {
            ParserContext = new ParserContext();
            _source.Process(ParserContext);

            return new ParsedData
            {
                Errors = ParserContext.Errors,
                Rows = ParserContext.Rows,
                InvalidRows = ParserContext.InvalidRows,
                Header = ParserContext.Header,
                Trailer = ParserContext.Trailer
            };
        }
    }
}
