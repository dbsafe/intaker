using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
using System;
using System.IO;

namespace DataProcessor.DataSource.File
{
    public class FileDataSourceConfig
    {
        public string Delimiter { get; set; }
        public bool HasFieldsEnclosedInQuotes { get; set; }
        public string Path { get; set; }
    }

    public class FileDataSource : IDataSource
    {
        private readonly FileDataSourceConfig _config;
        private readonly LineParser _lineParser;

        public event EventHandler<ProcessRowEventArgs> BeforeProcessRow;
        public event EventHandler<ProcessRowEventArgs> AfterProcessRow;
        public event EventHandler<ProcessFieldEventArgs> ProcessField;

        public FileDataSource(FileDataSourceConfig config)
        {
            _config = config;
            _lineParser = new LineParser
            {
                Delimiter = _config.Delimiter,
                HasFieldsEnclosedInQuotes = _config.HasFieldsEnclosedInQuotes
            };
        }

        public void Process(ParserContext context)
        {
            if (!System.IO.File.Exists(_config.Path))
            {
                throw new FileNotFoundException("File not found", _config.Path);
            }

            using (var reader = new StreamReader(_config.Path))
            {
                string nextLine = reader.ReadLine();
                while (nextLine != null && !context.IsAborted)
                {
                    Debug($"Parsing Line: '{nextLine}'");
                    context.CurrentRowRaw = nextLine;
                    nextLine = reader.ReadLine();
                    context.IsCurrentRowTheLast = nextLine == null;
                    CreateRow(context);
                    if (!context.IsCurrentRowTheLast)
                    {
                        context.CurrentRowIndex++;
                    }
                }
            }
        }

        private void CreateRow(ParserContext context)
        {
            var row = new Row
            {
                Index = context.CurrentRowIndex,
                Raw = context.CurrentRowRaw,
                RawFields = _lineParser.Parse(context.CurrentRowRaw),
                ValidationResult = ValidationResultType.Valid
            };

            context.CurrentRowRawFields = row.RawFields;

            var processRowEventArgs = new ProcessRowEventArgs(row, context);

            OnBeforeProcessRow(processRowEventArgs);
            if (context.IsAborted)
            {
                return;
            }

            CreateFieldsForRow(row, context);
            if (context.IsAborted)
            {
                return;
            }

            OnAfterProcessRow(processRowEventArgs);
        }

        private void CreateFieldsForRow(Row row, ParserContext context)
        {
            for (int fieldIndex = 0; fieldIndex < context.CurrentRowRawFields.Length; fieldIndex++)
            {
                var field = new Field
                {
                    Index = fieldIndex,
                    Raw = context.CurrentRowRawFields[fieldIndex],
                    Row = row,
                    ValidationResult = ValidationResultType.Valid
                };

                row.Fields.Add(field);
                OnFieldCreated(new ProcessFieldEventArgs(field, context));
                if (context.IsAborted)
                {
                    return;
                }
            }
        }

        protected virtual void OnBeforeProcessRow(ProcessRowEventArgs e)
        {
            BeforeProcessRow?.Invoke(this, e);
        }

        protected virtual void OnAfterProcessRow(ProcessRowEventArgs e)
        {
            AfterProcessRow?.Invoke(this, e);
        }

        protected virtual void OnFieldCreated(ProcessFieldEventArgs e)
        {
            ProcessField?.Invoke(this, e);
        }

        private void Debug(string message)
        {
            Domain.Utils.DataProcessorGlobal.Debug($"{nameof(FileDataSource)} - '{message}'");
        }
    }
}
