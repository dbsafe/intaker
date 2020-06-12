using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
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
            return new Models.FieldProcessorDefinition
            {
                FieldName = fieldDefinition.Name,
                Description = fieldDefinition.Description,
                Decoder = CreateDecoder(fieldDefinition),
                Rules = CreateRules(fieldDefinition),
                Aggregators = CreateAggregators(fieldDefinition)
            };
        }

        private static IFieldAggregator[] CreateAggregators(FieldDefinition fieldDefinition)
        {
            var fieldAggregators = new List<IFieldAggregator>();

            if (fieldDefinition.Aggregators?.Length > 0)
            {
                foreach (var aggregatorDefinition in fieldDefinition.Aggregators)
                {
                    fieldAggregators.Add(CreateAggregator(aggregatorDefinition));
                }
            }

            return fieldAggregators.ToArray();
        }

        private static IFieldAggregator CreateAggregator(AggregatorDefinition aggregatorDefinition)
        {
            var aggregator = StoreManager.AggregatorStore.CreateObject(aggregatorDefinition.Aggregator);
            aggregator.Description = aggregatorDefinition.Description;
            aggregator.Name = aggregatorDefinition.Name;
            return aggregator;
        }

        private static IFieldRule[] CreateRules(FieldDefinition fieldDefinition)
        {
            var fieldRules = new List<IFieldRule>();

            if (fieldDefinition.Rules?.Length > 0)
            {
                foreach (var ruleDefinition in fieldDefinition.Rules)
                {
                    fieldRules.Add(CreateRule(ruleDefinition));
                }
            }

            return fieldRules.ToArray();
        }

        private static IFieldRule CreateRule(RuleDefinition ruleDefinition)
        {
            var rule = StoreManager.RuleStore.CreateObject(ruleDefinition.Rule);
            rule.Args = ruleDefinition.Args;
            rule.Description = ruleDefinition.Description;
            rule.FailValidationResult = ruleDefinition.IsFixable ? ValidationResultType.InvalidFixable : ValidationResultType.InvalidCritical;
            rule.Name = ruleDefinition.Name;
            return rule;
        }

        private static IFieldDecoder CreateDecoder(FieldDefinition fieldDefinition)
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
                decoder.FailValidationResult = fieldDefinition.IsFixable ? ValidationResultType.InvalidFixable : ValidationResultType.InvalidCritical;
            }

            return decoder;
        }
    }
}
