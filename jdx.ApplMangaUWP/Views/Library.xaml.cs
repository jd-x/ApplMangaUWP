using jdx.ApplMangaUWP.Services.Navigation;
using jdx.ApplMangaUWP.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace jdx.ApplMangaUWP.Views {
    public sealed partial class Library : Page, IPageWithViewModel<LibraryViewModel> {
        private static int _persistedItemIndex = -1;

        public LibraryViewModel ViewModel { get; set; }

        public Library() {
            this.InitializeComponent();

            // Load manga collection grid items
        }

        public void UpdateBindings() {
            Bindings.Update();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back) {

            } else {
                _persistedItemIndex = -1;
            }

            Canvas.SetZIndex(this, 0);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            base.OnNavigatingFrom(e);
            Canvas.SetZIndex(this, 1);
        }
    }
}
