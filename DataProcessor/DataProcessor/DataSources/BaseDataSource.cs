using DataProcessor.Contracts;
using DataProcessor.Models;
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

    public abstract class BaseDataSource<TParserContext> : IDataSource<TParserContext>
        where TParserContext : IParserContext
    {
        private readonly LineParser _lineParser;

        public event EventHandler<ProcessFieldEventArgs<TParserContext>> ProcessField;
        public event EventHandler<ProcessRowEventArgs<TParserContext>> BeforeProcessRow;
        public event EventHandler<ProcessRowEventArgs<TParserContext>> AfterProcessRow;

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

        public void Process(TParserContext context)
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

        protected virtual void OnBeforeProcessRow(ProcessRowEventArgs<TParserContext> e)
        {
            BeforeProcessRow?.Invoke(this, e);
        }

        protected virtual void OnAfterProcessRow(ProcessRowEventArgs<TParserContext> e)
        {
            AfterProcessRow?.Invoke(this, e);
        }

        protected virtual void OnFieldCreated(ProcessFieldEventArgs<TParserContext> e)
        {
            ProcessField?.Invoke(this, e);
        }

        protected void Debug(string message)
        {
            Utils.DataProcessorGlobal.Debug($"{Name} - {message}");
        }

        private void CreateRow(TParserContext context)
        {
            var row = new Row
            {
                Index = context.CurrentRowIndex,
                Raw = context.CurrentRowRaw,
                RawFields = _lineParser.Parse(context.CurrentRowRaw),
                ValidationResult = ValidationResultType.Valid
            };

            context.CurrentRowRawFields = row.RawFields;

            var processRowEventArgs = new ProcessRowEventArgs<TParserContext>(row, context);

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

        private void CreateFieldsForRow(Row row, TParserContext context)
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
                OnFieldCreated(new ProcessFieldEventArgs<TParserContext>(field, context));
                if (context.IsAborted)
                {
                    return;
                }
            }
        }
    }
}
