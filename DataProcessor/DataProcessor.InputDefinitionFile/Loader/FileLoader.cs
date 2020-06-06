using System.IO;

namespace DataProcessor.InputDefinitionFile
{
    public static class FileLoader
    {
        public static T Load<T>(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File '{path}' not found", path);
            }

            var inputXml = File.ReadAllText(path);
            return HelperXmlSerializer.Deserialize<T>(inputXml);
        }
    }
}
