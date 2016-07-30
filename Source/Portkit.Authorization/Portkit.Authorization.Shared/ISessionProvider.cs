using System.Threading.Tasks;

namespace Portkit.Authorization
{
    public interface ISessionProvider
    {
        string Id { get; }

        Task LoginAsync();

        Task LogoutAsync();

        ISession GetSession();
    }
}