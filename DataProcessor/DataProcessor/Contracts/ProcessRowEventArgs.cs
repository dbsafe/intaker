using DataProcessor.Models;

namespace DataProcessor.Contracts
{
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

    public class ProcessRowEventArgs<TParserContext>
    {
        public TParserContext Context { get; }
        public Row Row { get; }

        public ProcessRowEventArgs(Row row, TParserContext context)
        {
            Row = row;
            Context = context;
        }
    }
}
