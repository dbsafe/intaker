using DataProcessor.Domain.Models;
using System;

namespace DataProcessor.Domain.Contracts
{
    public interface IDataSource
    {
        void Process(ParserContext context);

        event EventHandler<ProcessFieldEventArgs> ProcessField;
        event EventHandler<ProcessRowEventArgs> BeforeProcessRow;
        event EventHandler<ProcessRowEventArgs> AfterProcessRow;
    }

    public class ProcessFieldEventArgs
    {
        public Field Field { get; }
        public ParserContext Context { get; }

        public ProcessFieldEventArgs(Field field, ParserContext context)
        {
            Field = field;
            Context = context;
        }
    }

    public class ProcessRowEventArgs
    {
        public ParserContext Context { get; }
        public Row Row { get; }

        public ProcessRowEventArgs(Row row, ParserContext context)
        {
            Row = row;
            Context = context;
        }
    }
}
