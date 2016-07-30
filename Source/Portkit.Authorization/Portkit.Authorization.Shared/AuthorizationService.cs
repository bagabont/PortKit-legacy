using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portkit.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IList<ISessionProvider> _providers;

        /// <summary>
        /// Gets the <see cref="ISessionProvider"/> provider.
        /// </summary>
        public ISessionProvider Provider { get; private set; }

        public AuthorizationService(IList<ISessionProvider> providers)
        {
            if (providers == null || !providers.Any())
            {
                throw new ArgumentException("Must contain at least one valid provider.", nameof(providers));
            }
            _providers = providers;
        }

        /// <summary>
        /// Log in using the given provider type and save the session to the store.
        /// </summary>
        /// <typeparam name="TProvider">Type of <see cref="ISessionProvider"/> to be used for authorization.</typeparam>
        public async Task LoginAsync<TProvider>() where TProvider : ISessionProvider
        {
            Provider = _providers.OfType<TProvider>().FirstOrDefault();
            if (Provider == null)
            {
                throw new ArgumentException($"Cannot find provider of type {typeof(TProvider)}", nameof(TProvider));
            }
            await Provider.LoginAsync();
        }

        /// <summary>
        /// Logs out and clear session from store
        /// </summary>
        public async Task LogoutAsync(ISession session)
        {
            var provider = _providers.FirstOrDefault(p => p.Id == session.ProviderId);
            if (provider == null)
            {
                throw new IndexOutOfRangeException($"Cannot find provider with ID: '{session.ProviderId}'.");
            }
            await provider.LogoutAsync();
        }
    }
}