using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Portkit.ComponentModel
{
    public class ViewBase : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (DataContext as INavigationAware)?.OnNavigatedTo(e);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            (DataContext as INavigationAware)?.OnNavigatedFrom(e);
            if (e.NavigationMode != NavigationMode.Back)
            {
                return;
            }

            // Clear cache mode
            NavigationCacheMode = NavigationCacheMode.Disabled;
            (DataContext as IDisposable)?.Dispose();
        }
    }
}