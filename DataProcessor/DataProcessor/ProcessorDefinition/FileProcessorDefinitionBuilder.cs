using DataProcessor.Contracts;
using DataProcessor.Models;
using DataProcessor.InputDefinitionFile;
using DataProcessor.InputDefinitionFile.Models;
using DataProcessor.ObjectStore;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.ProcessorDefinition
{
    public static class FileProcessorDefinitionBuilder
    {
        public static Models.FileProcessorDefinition CreateFileProcessorDefinition(InputDefinitionFile_10 inputDefinitionFile_10)
        {
            var aggregateManager = new AggregateManager();
            var processorDefinition = new Models.FileProcessorDefinition
            {
                CreateRowJsonEnabled = inputDefinitionFile_10.CreateRowJsonEnabled,
                HeaderRowProcessorDefinition = LoadRowProcessorDefinition(inputDefinitionFile_10.Header, aggregateManager),
                DataRowProcessorDefinition = LoadRowProcessorDefinition(inputDefinitionFile_10.Data, aggregateManager),
                TrailerRowProcessorDefinition = LoadRowProcessorDefinition(inputDefinitionFile_10.Trailer, aggregateManager)
            };

            InitializeRules(processorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions.SelectMany(a => a.Rules), aggregateManager.GetAggregates());
            InitializeRules(processorDefinition.DataRowProcessorDefinition.FieldProcessorDefinitions.SelectMany(a => a.Rules), aggregateManager.GetAggregates());
            InitializeRules(processorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions.SelectMany(a => a.Rules), aggregateManager.GetAggregates());

            return processorDefinition;
        }

        private static void InitializeRules(IEnumerable<IFieldRule> rules, IEnumerable<Aggregate> aggregates)
        {
            var config = new FieldRuleConfiguration { Aggregates = aggregates };
            foreach (var rule in rules)
            {
                rule.Initialize(config);
            }
        }

        private static Models.RowProcessorDefinition LoadRowProcessorDefinition(RowDefinition rowDefinition, AggregateManager aggregateManager)
        {
            var fieldProcessorDefinitions = new List<Models.FieldProcessorDefinition>();

            if (rowDefinition != null)
            {
                foreach (var fieldDefinition in rowDefinition.Fields)
                {
                    var fieldProcessorDefinition = LoadFieldProcessorDefinition(fieldDefinition, aggregateManager);
                    fieldProcessorDefinitions.Add(fieldProcessorDefinition);
                }
            }

            return new Models.RowProcessorDefinition
            {
                FieldProcessorDefinitions = fieldProcessorDefinitions.ToArray()
            };
        }

        private static Models.FieldProcessorDefinition LoadFieldProcessorDefinition(FieldDefinition fieldDefinition, AggregateManager aggregateManager)
        {
            return new Models.FieldProcessorDefinition
            {
                FieldName = fieldDefinition.Name,
                Description = fieldDefinition.Description,
                Decoder = CreateDecoder(fieldDefinition),
                Rules = CreateRules(fieldDefinition),
                Aggregators = CreateAggregators(fieldDefinition, aggregateManager)                
            };
        }

        private static IFieldAggregator[] CreateAggregators(FieldDefinition fieldDefinition, AggregateManager aggregateManager)
        {
            var fieldAggregators = new List<IFieldAggregator>();

            if (fieldDefinition.Aggregators?.Length > 0)
            {
                foreach (var aggregatorDefinition in fieldDefinition.Aggregators)
                {
                    fieldAggregators.Add(CreateAggregator(aggregatorDefinition, aggregateManager));
                }
            }

            return fieldAggregators.ToArray();
        }

        private static IFieldAggregator CreateAggregator(AggregatorDefinition aggregatorDefinition, AggregateManager aggregateManager)
        {
            var aggregator = StoreManager.AggregatorStore.CreateObject(aggregatorDefinition.Aggregator);
            aggregator.Description = aggregatorDefinition.Description;
            aggregator.Name = aggregatorDefinition.Name;
            aggregator.Aggregate = aggregateManager.GetAggregateByName(aggregator.Name);

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
            rule.FailValidationResult = ruleDefinition.FailValidationResult;
            rule.Name = ruleDefinition.Name;
            return rule;
        }

        private static IFieldDecoder CreateDecoder(FieldDefinition fieldDefinition)
        {
            IFieldDecoder decoder;
            if (string.IsNullOrEmpty(fieldDefinition.Decoder))
            {
                decoder = new BypassDecoder
                {
                    FailValidationResult = ValidationResultType.Valid
                };
            }
            else
            {
                decoder = StoreManager.DecoderStore.CreateObject(fieldDefinition.Decoder);
                decoder.Pattern = fieldDefinition.Pattern;
                decoder.FailValidationResult = fieldDefinition.FailValidationResult;
            }

            return decoder;
        }
    }
}
