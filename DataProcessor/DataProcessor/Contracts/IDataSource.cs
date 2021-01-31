using System;

namespace DataProcessor.Contracts
{
    public interface IDataSource<TParserContext>
    {
        void Process(TParserContext context);

        event EventHandler<ProcessFieldEventArgs<TParserContext>> ProcessField;
        event EventHandler<ProcessRowEventArgs<TParserContext>> BeforeProcessRow;
        event EventHandler<ProcessRowEventArgs<TParserContext>> AfterProcessRow;
    }
}
