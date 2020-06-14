using DataProcessor.DataSource.File;
using DataProcessor.Domain.Contracts;

namespace DataProcessor
{
    public class FileDataLoaderConfig
    {
        public string Delimiter { get; set; }
        public bool HasFieldsEnclosedInQuotes { get; set; }
    }

    public class FileDataLoader
    {
        private readonly IDataRepository _dataRepository;
        private readonly FileDataLoaderConfig _config;

        public FileDataLoader(FileDataLoaderConfig config, IDataRepository dataRepository)
        {
            _config = config;
            _dataRepository = dataRepository;
        }

        public void Load(string dataFilePath, string fileSpecPath)
        {
            var fileDataSource = CreateDataSource(dataFilePath);

            // var parsedDataProcessor = new ParsedDataProcessor(fileDataSource);
        }

        private IDataSource CreateDataSource(string path)
        {
            var fileDataSourceConfig = new FileDataSourceConfig
            {
                Delimiter = _config.Delimiter,
                HasFieldsEnclosedInQuotes = _config.HasFieldsEnclosedInQuotes,
                Path = path
            };

            return new FileDataSource(fileDataSourceConfig);
        }
    }
}
