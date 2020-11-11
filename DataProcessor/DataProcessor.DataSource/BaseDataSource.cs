using DataProcessor.Domain.Contracts;
using DataProcessor.Domain.Models;
using System;

namespace DataProcessor.DataSource
{
    public interface IDataSourceConfig
    {
        string Delimiter { get; }
        bool HasFieldsEnclosedInQuotes { get; }
    }

    public interface ILineProvider : IDisposable
    {
        string ReadLine();
    }

    public abstract class BaseDataSource : IDataSource
    {
        private readonly LineParser _lineParser;

        public event EventHandler<ProcessFieldEventArgs> ProcessField;
        public event EventHandler<ProcessRowEventArgs> BeforeProcessRow;
        public event EventHandler<ProcessRowEventArgs> AfterProcessRow;

        public abstract string Name { get; }

        public BaseDataSource(IDataSourceConfig config)
        {
            _lineParser = new LineParser
            {
                Delimiter = config.Delimiter,
                HasFieldsEnclosedInQuotes = config.HasFieldsEnclosedInQuotes
            };
        }

        protected abstract ILineProvider CreateLineProvider();

        public void Process(ParserContext context)
        {
            using (var reader = CreateLineProvider())
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

        protected void Debug(string message)
        {
            Domain.Utils.DataProcessorGlobal.Debug($"{Name} - {message}");
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

            if (row.ValidationResult == ValidationResultType.Valid || row.ValidationResult == ValidationResultType.Warning)
            {
                CreateFieldsForRow(row, context);
                if (context.IsAborted)
                {
                    return;
                }
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
    }
}
