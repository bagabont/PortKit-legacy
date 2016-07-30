using System.IO;
using System.Xml.Serialization;

namespace Portkit.Authorization
{
    internal sealed class SessionSerializer : ISessionSerializer
    {
        public TSession DeserializeSession<TSession>(string data) where TSession : ISession
        {
            using (var textWriter = new StringReader(data))
            {
                var xmlSerializer = new XmlSerializer(typeof(TSession));
                return (TSession)xmlSerializer.Deserialize(textWriter);
            }
        }

        public string SerializeSession(ISession session)
        {
            using (var textWriter = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(session.GetType());
                xmlSerializer.Serialize(textWriter, session);
                return textWriter.ToString();
            }
        }
    }
}