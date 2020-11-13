namespace DataProcessor.DataSource.InMemory
{
    public class InMemoryDataSourceConfig : IDataSourceConfig
    {
        public string Delimiter { get; set; }
        public bool HasFieldsEnclosedInQuotes { get; set; }
    }

    public class InMemoryDataSource : BaseDataSource
    {
        private readonly string[] _lines;

        public override string Name => nameof(InMemoryDataSource);

        public InMemoryDataSource(InMemoryDataSourceConfig config, string[] lines)
            : base(config)
        {
            _lines = lines;
        }

        protected override ILineProvider CreateLineProvider() => new ArrayLineProvider(_lines);
    }

    public class ArrayLineProvider : ILineProvider
    {
        private readonly string[] _lines;
        private int _index;

        public ArrayLineProvider(string[] lines)
        {
            _lines = lines;
        }

        public void Dispose() { }

        public string ReadLine()
        {
            if (_index == _lines.Length)
            {
                return null;
            }

            return _lines[_index++];
        }
    }
}
