using System;
using System.Diagnostics;
using System.Linq;
using Windows.Security.Credentials;
using Portkit.Utils.Extensions;

namespace Portkit.Authorization
{
    /// <summary>
    /// Represents a wrapper around <see cref="PasswordVault"/> to securely store credentials.
    /// </summary>
    public class CredentialsLocker : ICredentialsLocker
    {
        private readonly string _resource;
        private readonly PasswordVault _vault;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// Creates a new instance of <see cref="CredentialsLocker"/> as "default-credentials-locker" resource.
        /// </summary>
        public CredentialsLocker() :
            this("default-credentials-locker")
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="CredentialsLocker"/>.
        /// </summary>
        /// <param name="resource">Resource name under which credentials will be stored.</param>
        public CredentialsLocker(string resource)
        {
            _resource = resource;
            _vault = new PasswordVault();
        }

        /// <summary>
        /// Saves the credentials to the storage.
        /// </summary>
        /// <param name="userName">Username.</param>
        /// <param name="password">Password.</param>
        [DebuggerStepThrough]
        public void Save(string userName, string password)
        {
            lock (_syncLock)
            {
                Delete();
                _vault.Add(new PasswordCredential(_resource, userName, password));
            }
        }

        /// <summary>
        /// Clears stored credentials.
        /// </summary>
        /// <returns>If successfully removed credentials, returns true, otherwise false.</returns>
        [DebuggerStepThrough]
        public bool Delete()
        {
            lock (_syncLock)
            {
                try
                {
                    var credentials = _vault.FindAllByResource(_resource);
                    if (credentials.IsNullOrEmpty())
                    {
                        return false;
                    }
                    foreach (var credential in credentials)
                    {
                        _vault.Remove(credential);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks if there are any credentials stored.
        /// </summary>
        /// <returns>True if credentials are found, otherwise false.</returns>
        [DebuggerStepThrough]
        public bool CheckIsEmpty()
        {
            lock (_syncLock)
            {
                try
                {
                    return _vault.FindAllByResource(_resource).FirstOrDefault() == null;
                }
                catch
                {
                    // If there are no credentials stored,
                    // the vault will throw an exception
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets the stored <see cref="PasswordCredential"/>.
        /// </summary>
        /// <returns>Instance of <see cref="PasswordCredential"/></returns>
        [DebuggerStepThrough]
        public PasswordCredential GetCredentials()
        {
            lock (_syncLock)
            {
                try
                {
                    var credential = _vault.FindAllByResource(_resource).FirstOrDefault();
                    credential.RetrievePassword();
                    return credential;
                }
                catch (Exception)
                {
                    // this exception likely means 
                    // that no credentials have been stored
                    return null;
                }
            }
        }

    }
}
