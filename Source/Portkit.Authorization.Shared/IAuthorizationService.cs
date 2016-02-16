using System.Threading.Tasks;

namespace Portkit.Authorization
{
    public interface IAuthorizationService
    {
        Task LoginAsync<TProvider>() where TProvider : ISessionProvider;

        Task LogoutAsync(ISession session);
    }
}
