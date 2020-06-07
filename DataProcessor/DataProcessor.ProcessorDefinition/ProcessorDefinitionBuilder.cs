using DataProcessor.Domain.Contracts;
using DataProcessor.InputDefinitionFile;
using DataProcessor.InputDefinitionFile.Models;
using DataProcessor.ObjectStore;
using System.Collections.Generic;

namespace DataProcessor.ProcessorDefinition
{
    public static class ProcessorDefinitionBuilder
    {
        public static Models.ProcessorDefinition CreateProcessorDefinition(InputDefinitionFile_10 inputDefinitionFile_10)
        {
            return new Models.ProcessorDefinition
            {
                HeaderRowProcessorDefinition = LoadRowProcessorDefinition(inputDefinitionFile_10.Header),
                DataRowProcessorDefinition = LoadRowProcessorDefinition(inputDefinitionFile_10.Data),
                TrailerRowProcessorDefinition = LoadRowProcessorDefinition(inputDefinitionFile_10.Trailer)
            };
        }

        private static Models.RowProcessorDefinition LoadRowProcessorDefinition(RowDefinition rowDefinition)
        {
            var fieldProcessorDefinitions = new List<Models.FieldProcessorDefinition>();

            if (rowDefinition != null)
            {
                foreach (var fieldDefinition in rowDefinition.Fields)
                {
                    var fieldProcessorDefinition = LoadFieldProcessorDefinition(fieldDefinition);
                    fieldProcessorDefinitions.Add(fieldProcessorDefinition);
                }
            }

            return new Models.RowProcessorDefinition
            {
                FieldProcessorDefinitions = fieldProcessorDefinitions.ToArray()
            };
        }

        private static Models.FieldProcessorDefinition LoadFieldProcessorDefinition(FieldDefinition fieldDefinition)
        {
            IFieldDecoder decoder;
            if (string.IsNullOrEmpty(fieldDefinition.Decoder))
            {
                decoder = new BypassDecoder();
            }
            else
            {
                decoder = StoreManager.DecoderStore.CreateObject(fieldDefinition.Decoder);
                decoder.Pattern = fieldDefinition.Pattern;
            }

            return new Models.FieldProcessorDefinition
            {
                FieldName = fieldDefinition.Name,
                Decoder = decoder
            };
        }
    }
}
