using Portkit.Utils;

namespace Portkit.Authorization
{
    public class SessionSerializer : ISessionSerializer
    {
        public TSession DeserializeSession<TSession>(string data) where TSession : ISession
        {
            return new GenericXmlSerializer().Deserialize<TSession>(data);
        }

        public string SerializeSession(ISession session)
        {
            return new GenericXmlSerializer().Serialize(session);
        }
    }
}
