using System;
using Windows.Foundation.Collections;
using Windows.Storage;
using Portkit.Utils.Extensions;

namespace Portkit.Authorization
{
    /// <summary>
    /// Represents a container storage class, for handling sensitive session data.
    /// </summary>
    public class SessionStore<TSession> : ISessionStore<TSession> where TSession : ISession
    {
        private const string DEFAULT_ENCRYPTION_KEY =
            "jc6szKQNv?m52FHQ!FC=N=jZ%mm*2%A#4aH_N5&9C9ApHEWmb-8gu$tEGq^Dtk*&5A7*C8$wgaWP8r44VSM+ppDskLK2w?5L!z27cc7XxQNJYjt8wKdfr=_NDDG*6+FD";

        private readonly ICredentialsLocker _locker;
        private readonly ISessionSerializer _serializer;
        private readonly IPropertySet _store;

        /// <summary>
        /// Creates an instance of <see cref="SessionStore{TSession}"/> class.
        /// </summary>
        public SessionStore() :
            this(new CredentialsLocker(), new SessionSerializer())
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="SessionStore{TSession}"/> class.
        /// </summary>
        /// <param name="locker">A <see cref="ICredentialsLocker"/> instance,
        ///  which provides secured storage for sensitive data.</param>
        /// <param name="serializer">An implementation of <see cref="ISessionSerializer"/> for 
        /// serializing and deserializing session data.</param>
        public SessionStore(ICredentialsLocker locker, ISessionSerializer serializer)
        {
            _locker = locker;
            _serializer = serializer;
            var localSettings = ApplicationData.Current.LocalSettings;
            var container = localSettings.CreateContainer("session-container", ApplicationDataCreateDisposition.Always);
            _store = container.Values;
        }

        /// <summary>
        /// Stores a session securely in the underlying storage.
        /// </summary>
        /// <param name="session"></param>
        public void SaveSession(TSession session)
        {
            // Clear any previous session
            Clear();

            // Serialize and encrypt the session, then save it
            var plainTextSession = _serializer.SerializeSession(session);
            _store["session"] = plainTextSession.Encrypt(GetKey());
        }

        /// <summary>
        /// Gets any stored session from the storage.
        /// </summary>
        /// <returns>Stored <see cref="ISession"/> object.</returns>
        public virtual TSession GetSession()
        {
            try
            {
                var cipherTextSession = (string)_store["session"];
                if (string.IsNullOrWhiteSpace(cipherTextSession))
                {
                    return default(TSession);
                }

                // Decrypt and deserialize session
                var plainTextSession = cipherTextSession.Decrypt(GetKey());
                return _serializer.DeserializeSession<TSession>(plainTextSession);
            }
            catch (Exception)
            {
                return default(TSession);
            }
        }

        /// <summary>
        /// Clears any saved session from the underlying storage.
        /// </summary>
        public void Clear()
        {
            _store.Clear();
        }

        private string GetKey()
        {
            // Try to get credentials
            var credential = _locker.GetCredentials();

            // Generate key by appending credentials to the default key.
            return $"{DEFAULT_ENCRYPTION_KEY}{credential?.Password ?? ""}";
        }
    }
}