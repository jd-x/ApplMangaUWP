using jdx.ApplMangaUWP.Services.Navigation;

namespace jdx.ApplMangaUWP.ViewModels {
    public class LibraryViewModel {
        private INavigationService _navigationService;

        public LibraryViewModel(INavigationService navigationService) => _navigationService = navigationService;
    }
}
