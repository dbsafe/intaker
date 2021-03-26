using DataProcessor;
using DataProcessor.DataSource.InStream;
using DataProcessor.InputDefinitionFile;
using DataProcessor.Models;
using System.IO;

namespace FileValidator.Domain.Services
{
    public interface IFileDecoder
    {
        ParsedDataAndSpec10 LoadVersion10(string content, string fileSpecXml);
        ParsedDataAndSpec20 LoadVersion20(string content, string fileSpecXml);
    }

    public class ParsedDataAndSpec10
    {
        public ParsedData10 ParsedData { get; set; }
        public InputDefinitionFile10 InputDefinitionFile { get; set; }
    }

    public class ParsedDataAndSpec20
    {
        public ParsedData20 ParsedData { get; set; }
        public InputDefinitionFile20 InputDefinitionFile { get; set; }
    }

    public class FileDecoder : IFileDecoder
    {
        public ParsedDataAndSpec10 LoadVersion10(string content, string fileSpecXml)
        {
            var inputDefinitionFile = FileLoader.LoadFromXml<InputDefinitionFile10>(fileSpecXml);
            var fileProcessorDefinition = DataProcessor.ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            var result = new ParsedDataAndSpec10 { InputDefinitionFile = inputDefinitionFile };

            var config = new StreamDataSourceConfig
            {
                Delimiter = inputDefinitionFile.Delimiter,
                HasFieldsEnclosedInQuotes = inputDefinitionFile.HasFieldsEnclosedInQuotes,
                CommentedOutIndicator = inputDefinitionFile.CommentedOutIndicator
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    var source = new StreamDataSource<ParserContext10>(config, stream);
                    var processor = new ParsedDataProcessor10(source, fileProcessorDefinition);
                    result.ParsedData = processor.Process();
                }
            }

            return result;
        }

        public ParsedDataAndSpec20 LoadVersion20(string content, string fileSpecXml)
        {
            var inputDefinitionFile = FileLoader.LoadFromXml<InputDefinitionFile20>(fileSpecXml);
            var fileProcessorDefinition = DataProcessor.ProcessorDefinition.FileProcessorDefinitionBuilder.CreateFileProcessorDefinition(inputDefinitionFile);

            var result = new ParsedDataAndSpec20 { InputDefinitionFile = inputDefinitionFile };

            var config = new StreamDataSourceConfig
            {
                Delimiter = inputDefinitionFile.Delimiter,
                HasFieldsEnclosedInQuotes = inputDefinitionFile.HasFieldsEnclosedInQuotes,
                CommentedOutIndicator = inputDefinitionFile.CommentedOutIndicator
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    var source = new StreamDataSource<ParserContext20>(config, stream);
                    var processor = new ParsedDataProcessor20(source, fileProcessorDefinition);
                    result.ParsedData = processor.Process();
                }
            }

            return result;
        }
    }
}
