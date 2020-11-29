using DataProcessor;
using DataProcessor.DataSource.InStream;
using DataProcessor.InputDefinitionFile;
using DataProcessor.InputDefinitionFile.Models;
using System.IO;

namespace FileValidator.Domain.Services
{
    public interface IFileDecoder
    {
        ParsedDataAndSpec Load(string content, string fileSpecXml);
    }

    public class ParsedDataAndSpec
    {
        public ParsedData ParsedData { get; set; }
        public InputDefinitionFile InputDefinitionFile { get; set; }
    }

    public class FileDecoder : IFileDecoder
    {
        public ParsedDataAndSpec Load(string content, string fileSpecXml)
        {
            var inputDefinitionFile = FileLoader.LoadFromXml<InputDefinitionFile_10>(fileSpecXml);
            var fileProcessorDefinition = DataProcessor.ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            var result = new ParsedDataAndSpec { InputDefinitionFile = inputDefinitionFile };

            var config = new StreamDataSourceConfig
            {
                Delimiter = inputDefinitionFile.Delimiter,
                HasFieldsEnclosedInQuotes = inputDefinitionFile.HasFieldsEnclosedInQuotes
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                    writer.Flush();

                    var source = new StreamDataSource(config, stream);
                    var processor = new ParsedDataProcessor(source, fileProcessorDefinition);
                    result.ParsedData = processor.Process();
                }
            }

            return result;
        }
    }
}
