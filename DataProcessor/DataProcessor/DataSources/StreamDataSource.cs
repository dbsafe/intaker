using DataProcessor.Models;
using System;
using System.IO;

namespace DataProcessor.DataSource.InStream
{
    public class StreamDataSourceConfig : IDataSourceConfig
    {
        public string Delimiter { get; set; }
        public bool HasFieldsEnclosedInQuotes { get; set; }
    }

    public class StreamDataSource<TParserContext> : BaseDataSource<TParserContext>
        where TParserContext : IParserContext
    {
        private readonly Stream _stream;

        public override string Name => "StreamDataSource";

        public StreamDataSource(StreamDataSourceConfig config, Stream stream) 
            : base(config)
        {
            _stream = stream;
        }

        protected override ILineProvider CreateLineProvider() => new StreamLineProvider(_stream);
    }

    public class StreamLineProvider : ILineProvider
    {
        private bool _disposed = false;
        private readonly StreamReader _reader;

        public StreamLineProvider(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            _reader = new StreamReader(stream);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _reader.Dispose();
                }

                _disposed = true;
            }
        }

        public string ReadLine() => _reader.ReadLine();
    }
}
