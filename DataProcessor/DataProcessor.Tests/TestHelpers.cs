using DataProcessor.DataSource.File;
using System.IO;
using System.Reflection;

namespace DataProcessor.Tests
{
    public static class TestHelpers
    {
        public static FileDataSource CreateFileDataSource(string filename, bool hasFieldsEnclosedInQuotes)
        {
            var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var config = new FileDataSourceConfig
            {
                Delimiter = ",",
                HasFieldsEnclosedInQuotes = hasFieldsEnclosedInQuotes,
                Path = Path.Combine(testDirectory, "TestFiles", filename)
            };

            return new FileDataSource(config);
        }
    }
}
