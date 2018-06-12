using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jdx.ApplMangaUWP.DataModel;

namespace jdx.ApplMangaUWP.Services.Navigation {
    public interface INavigationService {
        event EventHandler<bool> IsNavigatingChanged;

        event EventHandler Navigated;

        bool CanGoBack { get; }

        bool IsNavigating { get; }

        Task NavigateToLibraryAsync();

        Task GoBackAsync();
    }
}
