using Windows.Security.Credentials;

namespace Portkit.Authorization
{
    public interface ICredentialsLocker
    {
        PasswordCredential GetCredentials();

        bool CheckIsEmpty();

        bool Delete();

        void Save(string userName, string password);
    }
}