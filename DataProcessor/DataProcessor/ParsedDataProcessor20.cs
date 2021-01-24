using DataProcessor.Contracts;
using DataProcessor.ProcessorDefinition.Models;
using DataProcessor.Utils;
using System;
using System.Linq;

namespace DataProcessor
{
    public class ParsedDataProcessor20 : ParsedDataProcessor
    {
        private readonly FileProcessorDefinition20 _fileProcessorDefinition;

        public ParsedDataProcessor20(IDataSource source, FileProcessorDefinition20 fileProcessorDefinition) 
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
            rowProcessorDefinition = _fileProcessorDefinition.DataRowProcessorDefinitions;
            e.Context.DataRows.Add(e.Row);
            ValidateNumerOfFields(lineType, e.Row, rowProcessorDefinition);
        }

        private void SourceAfterProcessRow(object sender, ProcessRowEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SourceProcessField(object sender, ProcessFieldEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void ValidateProcessorDefinition(FileProcessorDefinition20 processorDefinition)
        {
            if (processorDefinition is null)
            {
                throw new ArgumentNullException(nameof(processorDefinition));
            }

            ValidateRowProcessorDefinition("Header", processorDefinition.HeaderRowProcessorDefinition);
            ValidateRowProcessorDefinition("Trailer", processorDefinition.TrailerRowProcessorDefinition);

            ValidateDataRowProcessorDefinitions(processorDefinition);
        }

        private static void ValidateDataRowProcessorDefinitions(FileProcessorDefinition20 processorDefinition)
        {
            if (processorDefinition.DataRowProcessorDefinitions is null)
            {
                throw new ArgumentNullException($"{nameof(processorDefinition.DataRowProcessorDefinitions)}");
            }

            ValidateKeyField(processorDefinition);

            foreach(var item in processorDefinition.DataRowProcessorDefinitions.ToArray())
            {
                ValidateRowProcessorDefinition(item.Key, item.Value);
            }
        }

        private static void ValidateKeyField(FileProcessorDefinition20 processorDefinition)
        {
            if (string.IsNullOrWhiteSpace(processorDefinition.KeyField))
            {
                throw new ArgumentNullException($"{nameof(processorDefinition.KeyField)} cannot be null or empty");
            }

            foreach(var value in processorDefinition.DataRowProcessorDefinitions.Values)
            {
                var keyFieldFound = value.FieldProcessorDefinitions.Any(a => a.FieldName == processorDefinition.KeyField);
                if (!keyFieldFound)
                {
                    throw new ArgumentNullException($"{nameof(processorDefinition.KeyField)} - A field with name '{processorDefinition.KeyField}' was not found in every data element");
                }
            }
        }
    }
}
