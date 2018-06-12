using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace jdx.ApplMangaUWP.Services.Navigation {
    public interface INavigableTo {
        /// <summary>
        /// The event that gets called by the Navigation Service after navigation has completed.
        /// </summary>
        /// <remarks>
        /// This gets called prior to <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs)"/>
        /// </remarks>
        /// <param name="navigationMode"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task NavigatedTo(NavigationMode navigationMode, object parameter);
    }
}