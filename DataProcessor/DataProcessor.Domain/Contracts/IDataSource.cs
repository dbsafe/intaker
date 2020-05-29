using DataProcessor.Domain.Models;
using System;

namespace DataProcessor.Domain.Contracts
{
    public interface IDataSource
    {
        void Process(ParserContext context);
        event EventHandler<ProcessFieldEventArgs> ProcessField;
        event EventHandler<ProcessRowEventArgs> ProcessRow;
    }

    public class ProcessFieldEventArgs
    {
        public Field Field { get; }

        public ProcessFieldEventArgs(Field field)
        {
            Field = field;
        }
    }

    public class ProcessRowEventArgs
    {
        public Row Row { get; }

        public ProcessRowEventArgs(Row row)
        {
            Row = row;
        }
    }
}
