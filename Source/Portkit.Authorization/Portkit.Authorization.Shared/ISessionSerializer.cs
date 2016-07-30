namespace Portkit.Authorization
{
    public interface ISessionSerializer
    {
        TSession DeserializeSession<TSession>(string data) where TSession : ISession;

        string SerializeSession(ISession session);
    }
}