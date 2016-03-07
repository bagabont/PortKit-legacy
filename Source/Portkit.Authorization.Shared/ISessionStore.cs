namespace Portkit.Authorization
{
    public interface ISessionStore<TSession> where TSession : ISession
    {
        void SaveSession(TSession session);

        TSession GetSession();

        void Clear();
    }
}