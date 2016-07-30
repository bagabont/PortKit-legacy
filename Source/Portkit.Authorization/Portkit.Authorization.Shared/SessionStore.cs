using System;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;

namespace Portkit.Authorization
{
    /// <summary>
    /// Represents a storage class, for handling sensitive session data.
    /// </summary>
    public abstract class SessionStore<TSession> : ISessionStore<TSession> where TSession : ISession
    {
        private const string DEFAULT_SECURITY_KEY = "jc6szKQNv?m52FHQ!FC=N=jZ%mm*2%A#4aH_N5&9C9ApHEWmb-8gu$tEGq^Dtk*&5A7*C8$wgaWP8r44VSM+ppDskLK2w?5L!z27cc7XxQNJYjt8wKdfr=_NDDG*6+FD";
        private readonly ICredentialsLocker _locker;
        private readonly ISessionSerializer _serializer;
        private readonly string _securityKey;
        private readonly IPropertySet _store;

        /// <summary>
        /// Creates an instance of <see cref="SessionStore{TSession}"/> class.
        /// </summary>
        protected SessionStore() :
            this(new CredentialsLocker(), new SessionSerializer(), DEFAULT_SECURITY_KEY)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="SessionStore{TSession}"/> class.
        /// </summary>
        /// <param name="locker">A <see cref="ICredentialsLocker"/> instance,
        ///  which provides secured storage for sensitive data.</param>
        /// <param name="serializer">An implementation of <see cref="ISessionSerializer"/> for 
        /// serializing and deserializing session data.</param>
        /// <param name="securityKey">Security key used to encrypt/decrypt session.</param>
        protected SessionStore(ICredentialsLocker locker, ISessionSerializer serializer, string securityKey)
        {
            _locker = locker;
            _serializer = serializer;
            _securityKey = securityKey;
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
            _store["session"] = Encrypt(plainTextSession, GetKey());
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
                var plainTextSession = Decrypt(cipherTextSession, GetKey());
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
            return $"{_securityKey}{credential?.Password ?? string.Empty}";
        }

        /// <summary>
        /// Encrypts text string.
        /// </summary>
        /// <param name="plainText">Plain text to encrypt.</param>
        /// <param name="key">Encryption key.</param>
        /// <returns>Encrypted text string.</returns>
        private static string Encrypt(string plainText, string key)
        {
            var keyHash = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var plainBuffer = CryptographicBuffer.ConvertStringToBinary(plainText, BinaryStringEncoding.Utf8);
            var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            var symetricKey = aes.CreateSymmetricKey(keyHash);
            var buffEncrypted = CryptographicEngine.Encrypt(symetricKey, plainBuffer, null);
            var cipherText = CryptographicBuffer.EncodeToBase64String(buffEncrypted);
            return cipherText;
        }

        /// <summary>
        /// Decrypts text string.
        /// </summary>
        /// <param name="cipherText">Cipher text to decrypt.</param>
        /// <param name="key">Encrypted key, used to encrypt the text string.</param>
        /// <returns>Plain text string.</returns>
        private static string Decrypt(string cipherText, string key)
        {
            var keyHash = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var cipherBuffer = CryptographicBuffer.DecodeFromBase64String(cipherText);
            var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            var symetricKey = aes.CreateSymmetricKey(keyHash);
            var buffDecrypted = CryptographicEngine.Decrypt(symetricKey, cipherBuffer, null);
            string plainText = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffDecrypted);
            return plainText;
        }
    }
}