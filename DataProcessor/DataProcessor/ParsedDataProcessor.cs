using DataProcessor.Contracts;
using DataProcessor.Models;

namespace DataProcessor
{
    public abstract class ParsedDataProcessor<TParserContext>
    {
        protected readonly IDataSource<TParserContext> _source;
        protected readonly bool _hasHeader;
        protected readonly bool _hasTrailer;

        public ParsedDataProcessor(IDataSource<TParserContext> source, bool hasHeader, bool hasTrailer)
        {
            _source = source;
            _hasHeader = hasHeader;
            _hasTrailer = hasTrailer;
        }

        protected bool IsHeaderRow(Row row)
        {
            return row.Index == 0 && _hasHeader;
        }

        protected bool IsTrailerRow(bool isCurrentRowTheLast)
        {
            return isCurrentRowTheLast && _hasTrailer;
        }
    }
}
