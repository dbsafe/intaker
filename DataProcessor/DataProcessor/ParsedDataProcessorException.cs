using System;

namespace DataProcessor
{
    public class ParsedDataProcessorException : Exception
    {
        public ParsedDataProcessorException(string message)
            : base(message)
        {
        }

        public ParsedDataProcessorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
