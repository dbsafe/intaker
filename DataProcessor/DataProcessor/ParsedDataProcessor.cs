using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.ProcessorDefinition.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DataProcessor
{
    public abstract class ParsedDataProcessor
    {
        protected readonly IDataSource _source;
        protected readonly bool _hasHeader;
        protected readonly bool _hasTrailer;

        public ParsedDataProcessor(IDataSource source, bool hasHeader, bool hasTrailer)
        {
            _source = source;
            _hasHeader = hasHeader;
            _hasTrailer = hasTrailer;
        }

        protected bool IsHeaderRow(Row row)
        {
            return row.Index == 0 && _hasHeader;
        }

        protected bool IsTrailerRow(bool isCurrentRowTheLast)
        {
            return isCurrentRowTheLast && _hasTrailer;
        }

        protected static void SetJson(Row row, FieldProcessorDefinition[] fieldProcessorDefinitions)
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

        protected static void ValidateRowProcessorDefinition(string lineType, RowProcessorDefinition rowProcessorDefinition)
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
    }
}
