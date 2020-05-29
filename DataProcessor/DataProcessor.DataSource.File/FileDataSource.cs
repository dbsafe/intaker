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

        public event EventHandler<ProcessFieldEventArgs> ProcessField;
        public event EventHandler<ProcessRowEventArgs> ProcessRow;

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
                while (nextLine != null && !context.Abort)
                {
                    Debug($"Parsing Line: '{nextLine}'");
                    context.CurrentRowRaw = nextLine;
                    nextLine = reader.ReadLine();
                    context.IsCurrentRowTheLast = nextLine == null;
                    ParseLine(context);
                    if (!context.IsCurrentRowTheLast)
                    {
                        context.CurrentRowIndex++;
                    }
                }
            }
        }

        private void ParseLine(ParserContext context)
        {
            var row = new Row { Index = context.CurrentRowIndex, Raw = context.CurrentRowRaw };
            context.Rows.Add(row);

            var rawFields = _lineParser.Parse(row.Raw);
            for (int fieldIndex = 0; fieldIndex < rawFields.Length; fieldIndex++)
            {
                var rawField = rawFields[fieldIndex];
                Debug($"Parsing field: '{rawField}'");
                var field = new Field { Index = fieldIndex, Raw = rawField, RowIndex = row.Index };
                row.Fields.Add(field);
                OnProcessField(new ProcessFieldEventArgs(field));
                if (context.Abort)
                {
                    return;
                }
            }

            OnProcessRow(new ProcessRowEventArgs(row));
        }

        protected void OnProcessRow(ProcessRowEventArgs e)
        {
            ProcessRow?.Invoke(this, e);
        }

        protected void OnProcessField(ProcessFieldEventArgs e)
        {
            ProcessField?.Invoke(this, e);
        }

        private void Debug(string message)
        {
            Domain.Utils.DataProcessorGlobal.Debug($"{nameof(FileDataSource)} - '{message}'");
        }
    }
}
