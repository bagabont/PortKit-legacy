using Windows.UI.Xaml.Navigation;

namespace Portkit.ComponentModel
{
    public interface INavigationAware
    {
        void OnNavigatedTo(NavigationEventArgs args);

        void OnNavigatedFrom(NavigationEventArgs args);
    }
}
