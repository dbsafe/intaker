using DataProcessor.Models;
using System;
using System.IO;

namespace DataProcessor.DataSource.File
{
    public class FileDataSourceConfig : IDataSourceConfig
    {
        public string Delimiter { get; set; }
        public bool HasFieldsEnclosedInQuotes { get; set; }
        public string Path { get; set; }
    }

    public class FileDataSource<TParserContext> : BaseDataSource<TParserContext>
        where TParserContext : ParserContext
    {
        private readonly FileDataSourceConfig _config;

        public override string Name => "FileDataSource";

        public FileDataSource(FileDataSourceConfig config)
            : base(config)
        {
            _config = config;
        }

        protected override ILineProvider CreateLineProvider() => new FileLineProvider(_config.Path);
    }

    public class FileLineProvider : ILineProvider
    {
        private bool _disposed = false;
        private readonly StreamReader _reader;

        public FileLineProvider(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new FileNotFoundException("File not found", path);
            }

            _reader = new StreamReader(path);
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
