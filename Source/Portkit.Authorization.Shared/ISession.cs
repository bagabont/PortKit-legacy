using System;

namespace Portkit.Authorization
{
    public interface ISession
    {
        string ProviderId { get; }

        string AccessToken { get; }

        DateTime ExpirationDate { get; }

        bool CheckIsValid();
    }
}