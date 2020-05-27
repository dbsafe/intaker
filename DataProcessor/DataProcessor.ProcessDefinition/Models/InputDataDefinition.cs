using System.Xml.Serialization;

namespace DataProcessor.ProcessDefinition.Models
{
    public abstract class InputDataDefinition
    {
        private string frameworkVersion;

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlElement("header")]
        public RowDefinition Header { get; set; }

        [XmlElement("data")]
        public RowDefinition Data { get; set; }

        [XmlElement("trailer")]
        public RowDefinition Trailer { get; set; }

        [XmlAttribute("frameworkVersion")]
        public string FrameworkVersion
        {
            get => frameworkVersion;
            set
            {
                frameworkVersion = value;
                OnFrameworkVersionSet(frameworkVersion);
            }
        }

        protected virtual void OnFrameworkVersionSet(string frameworkVersion)
        {
        }
    }
}
