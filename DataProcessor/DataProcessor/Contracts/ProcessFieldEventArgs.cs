using DataProcessor.Models;

namespace DataProcessor.Contracts
{
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
