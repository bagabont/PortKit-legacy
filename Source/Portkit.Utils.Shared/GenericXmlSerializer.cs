using System.IO;
using System.Xml.Serialization;

namespace Portkit.Utils
{
    public class GenericXmlSerializer
    {
        public TDeserialized Deserialize<TDeserialized>(string data)
        {
            var xmlSerializer = new XmlSerializer(typeof(TDeserialized));
            using (var textWriter = new StringReader(data))
            {
                return (TDeserialized)xmlSerializer.Deserialize(textWriter);
            }
        }

        public string Serialize(object model)
        {
            var xmlSerializer = new XmlSerializer(model.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, model);
                return textWriter.ToString();
            }
        }
    }
}
