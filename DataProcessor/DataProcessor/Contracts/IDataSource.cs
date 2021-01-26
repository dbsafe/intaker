using DataProcessor.Models;
using System;

namespace DataProcessor.Contracts
{
    public interface IDataSource
    {
        void Process(ParserContext context);

        event EventHandler<ProcessFieldEventArgs> ProcessField;
        event EventHandler<ProcessRowEventArgs> BeforeProcessRow;
        event EventHandler<ProcessRowEventArgs> AfterProcessRow;
    }

    public interface IDataSource<TParserContext>
    {
        void Process(TParserContext context);

        event EventHandler<ProcessFieldEventArgs<TParserContext>> ProcessField;
        event EventHandler<ProcessRowEventArgs<TParserContext>> BeforeProcessRow;
        event EventHandler<ProcessRowEventArgs<TParserContext>> AfterProcessRow;
    }
}
