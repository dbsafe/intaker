using DataProcessor.Contracts;
using DataProcessor.InputDefinitionFile;
using DataProcessor.InputDefinitionFile.Models;
using DataProcessor.Models;
using DataProcessor.ObjectStore;
using DataProcessor.ProcessorDefinition.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.ProcessorDefinition
{
    public static class FileProcessorDefinitionBuilder
    {
        public static FileProcessorDefinition10 CreateFileProcessorDefinition(InputDefinitionFile_10 inputDefinitionFile_10)
        {
            var aggregateManager = new AggregateManager();
            var processorDefinition = new FileProcessorDefinition10();
            InitializeFileProcessorDefinition(processorDefinition, inputDefinitionFile_10, aggregateManager);
            processorDefinition.DataRowProcessorDefinition = LoadRowProcessorDefinition(inputDefinitionFile_10.Data, aggregateManager);

            InitializeRules(processorDefinition.DataRowProcessorDefinition.FieldProcessorDefinitions.SelectMany(a => a.Rules), aggregateManager.GetAggregates());
            InitializeRules(processorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions.SelectMany(a => a.Rules), aggregateManager.GetAggregates());

            return processorDefinition;
        }

        public static FileProcessorDefinition20 CreateFileProcessorDefinition(InputDefinitionFile_20 inputDefinitionFile_20)
        {
            var aggregateManager = new AggregateManager();
            var processorDefinition = new FileProcessorDefinition20();
            InitializeFileProcessorDefinition(processorDefinition, inputDefinitionFile_20, aggregateManager);
            processorDefinition.DataRowProcessorDefinitions = LoadRowProcessorDefinitions(inputDefinitionFile_20.Datas, aggregateManager);
            processorDefinition.KeyField = inputDefinitionFile_20.Datas.KeyField;
            processorDefinition.DataTypeField = inputDefinitionFile_20.Datas.DataTypeField;

            ValidateKeyField(processorDefinition);
            ValidateDataTypeField(processorDefinition);

            var fieldProcessorDefinitionsInDataRows = processorDefinition.DataRowProcessorDefinitions.SelectMany(a => a.Value.FieldProcessorDefinitions);
            InitializeRules(fieldProcessorDefinitionsInDataRows.SelectMany(a => a.Rules), aggregateManager.GetAggregates());
            InitializeRules(processorDefinition.TrailerRowProcessorDefinition.FieldProcessorDefinitions.SelectMany(a => a.Rules), aggregateManager.GetAggregates());

            return processorDefinition;
        }

        private static void ValidateDataTypeField(FileProcessorDefinition20 processorDefinition)
        {
            if (string.IsNullOrWhiteSpace(processorDefinition.DataTypeField))
            {
                throw new InvalidOperationException("Invalid DataTypeField");
            }

            foreach (var RowProcessorDefinition in processorDefinition.DataRowProcessorDefinitions.Values)
            {
                var keyFieldFound = RowProcessorDefinition.FieldProcessorDefinitions.Any(a => a.FieldName == processorDefinition.DataTypeField);
                if (!keyFieldFound)
                {
                    throw new InvalidOperationException($"DataTypeField '{processorDefinition.DataTypeField}' must be present in every data definition");
                }
            }
        }

        private static void ValidateKeyField(FileProcessorDefinition20 processorDefinition)
        {
            if (string.IsNullOrWhiteSpace(processorDefinition.KeyField))
            {
                throw new InvalidOperationException("Invalid KeyField");
            }

            foreach(var RowProcessorDefinition in processorDefinition.DataRowProcessorDefinitions.Values)
            {
                var keyFieldFound = RowProcessorDefinition.FieldProcessorDefinitions.Any(a => a.FieldName == processorDefinition.KeyField);
                if (!keyFieldFound)
                {
                    throw new InvalidOperationException($"KeyField '{processorDefinition.KeyField}' must be present in every data definition");
                }
            }
        }

        private static void InitializeFileProcessorDefinition(
            FileProcessorDefinition fileProcessorDefinition,
            InputDefinitionFile.Models.InputDefinitionFile inputDefinitionFile,
            AggregateManager aggregateManager)
        {
            fileProcessorDefinition.CreateRowJsonEnabled = inputDefinitionFile.CreateRowJsonEnabled;
            fileProcessorDefinition.HeaderRowProcessorDefinition = LoadRowProcessorDefinition(inputDefinitionFile.Header, aggregateManager);
            fileProcessorDefinition.TrailerRowProcessorDefinition = LoadRowProcessorDefinition(inputDefinitionFile.Trailer, aggregateManager);

            InitializeRules(fileProcessorDefinition.HeaderRowProcessorDefinition.FieldProcessorDefinitions.SelectMany(a => a.Rules), aggregateManager.GetAggregates());
        }

        private static void InitializeRules(IEnumerable<IFieldRule> rules, IEnumerable<Aggregate> aggregates)
        {
            var config = new FieldRuleConfiguration { Aggregates = aggregates };
            foreach (var rule in rules)
            {
                rule.Initialize(config);
            }
        }

        private static Dictionary<string, RowProcessorDefinition> LoadRowProcessorDefinitions(Datas datasDefinitions, AggregateManager aggregateManager)
        {
            var result = new Dictionary<string, RowProcessorDefinition>();

            foreach (var rowDefinition in datasDefinitions.Rows)
            {
                var fieldProcessorDefinitions = LoadRowProcessorDefinition(rowDefinition, aggregateManager);
                if (result.ContainsKey(rowDefinition.DataType))
                {
                    throw new InvalidOperationException($"DataType '{rowDefinition.DataType}' is duplicated");
                }

                result[rowDefinition.DataType] = fieldProcessorDefinitions;
            }

            return result;
        }

        private static RowProcessorDefinition LoadRowProcessorDefinition(RowDefinition rowDefinition, AggregateManager aggregateManager)
        {
            var fieldProcessorDefinitions = new List<FieldProcessorDefinition>();

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

        private static FieldProcessorDefinition LoadFieldProcessorDefinition(FieldDefinition fieldDefinition, AggregateManager aggregateManager)
        {
            return new FieldProcessorDefinition
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
