using DataProcessor.Models;

namespace DataProcessor.Contracts
{
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
