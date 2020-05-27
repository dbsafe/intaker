using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace DataProcessor.ProcessDefinition.Utils
{
    public class HelperXmlSerializer
    {
        private static readonly XmlSerializerNamespaces _namespaces;

        static HelperXmlSerializer()
        {
            _namespaces = new XmlSerializerNamespaces();
            _namespaces.Add("", "");
        }

        public static string Serialize<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriterUtf8())
            {
                serializer.Serialize(writer, obj, _namespaces);
                return writer.ToString();
            }
        }

        public static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }

    public class StringWriterUtf8 : StringWriter
    {
        public override Encoding Encoding { get; } = Encoding.UTF8;
    }
}
