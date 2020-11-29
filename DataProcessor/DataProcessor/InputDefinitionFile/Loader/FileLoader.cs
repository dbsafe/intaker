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
            return LoadFromXml<T>(inputXml);
        }

        public static T LoadFromXml<T>(string xml)
        {
            return HelperXmlSerializer.Deserialize<T>(xml);
        }
    }
}
