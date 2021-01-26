using DataProcessor.Models;

namespace DataProcessor.Contracts
{
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

    public class ProcessFieldEventArgs<TParserContext>
    {
        public Field Field { get; }
        public TParserContext Context { get; }

        public ProcessFieldEventArgs(Field field, TParserContext context)
        {
            Field = field;
            Context = context;
        }
    }
}
