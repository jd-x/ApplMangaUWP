using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using jdx.ApplMangaUWP.DataModel;
using jdx.ApplMangaUWP.ViewModels;
using jdx.ApplMangaUWP.Views;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace jdx.ApplMangaUWP.Services.Navigation {
    public class NavigationService : INavigationService {
        private IFrameAdapter Frame { get; }
        private IComponentContext AutofacDependencyResolver { get; }
        private delegate Task NavigatedToViewModelDelegate(object page, object parameter, NavigationEventArgs navigationArgs);
        private Dictionary<Type, NavigatedToViewModelDelegate> PageViewModels { get; }

        private bool _isNavigating;
        public bool IsNavigating {
            get => _isNavigating;
            set {
                if(value != _isNavigating) {
                    _isNavigating = value;
                    IsNavigatingChanged?.Invoke(this, _isNavigating);

                    // Check that navigation just finished
                    if(!_isNavigating) {
                        // Done navigating
                        Navigated?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Return to the previously visited page
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public async Task GoBackAsync() {
            if(Frame.CanGoBack) {
                IsNavigating = true;

                Page navigatePage = await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    Frame.GoBack();
                    return Frame.Content as Page;
                });
            }
        }

        public bool CanGoBack => Frame.CanGoBack;

        public event EventHandler<bool> IsNavigatingChanged;

        public event EventHandler Navigated;

        // Collection -> local manga list
        public Task NavigateToLibraryAsync() => NavigateToPage<Library>();

        // Browse -> online manga list (cached to make loading faster)
        //public Task NavigateToBrowseAsync() => NavigateToPage<Browse>();

        // Downloads -> downloads list, selecting an item gives an option to open its online webpage or browse locally downloaded files
        //public Task NavigateToDownloadsAsync() => NavigateToPage<Downloads>();

        
        //public Task NavigateToInfoAsync(MangaEntry manga) => 

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        /// <param name="frameAdapter"></param>
        /// <param name="iocResolver"></param>
        public NavigationService(IFrameAdapter frameAdapter, IComponentContext iocResolver) {
            Frame = frameAdapter;
            AutofacDependencyResolver = iocResolver;

            // TODO: Move these mappings into the IOC container for multi platform use
            PageViewModels = new Dictionary<Type, NavigatedToViewModelDelegate>();
            RegisterPageViewModel<Library, LibraryViewModel>();
            Frame.Navigated += Frame_Navigated;
        }

        /// <summary>
        /// The Navigated event. This event is raised before <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedTo(NavigationEventArgs)"/>
        /// </summary>
        /// <param name="sender">The frame</param>
        /// <param name="e">The arguments coming from the frame</param>
        private void Frame_Navigated(object sender, NavigationEventArgs e) {
            IsNavigating = false;
            if(PageViewModels.ContainsKey(e.SourcePageType)) {
                var loadViewModelDelegate = PageViewModels[e.SourcePageType];
                var ignoredTask = loadViewModelDelegate(e.Content, e.Parameter, e);
            }
        }

        private void RegisterPageViewModel<TPage, TViewModel>()
            where TViewModel : class {
            NavigatedToViewModelDelegate navigatedTo = async(page, parameter, navArgs) => {
                if (page is IPageWithViewModel<TViewModel> pageWithVM) {
                    pageWithVM.ViewModel = AutofacDependencyResolver.Resolve<TViewModel>();

                    if(pageWithVM.ViewModel is INavigableTo navVM) {
                        await navVM.NavigatedTo(navArgs.NavigationMode, parameter);
                    }

                    // Async loading
                    pageWithVM.UpdateBindings();
                }
            };

            PageViewModels[typeof(TPage)] = navigatedTo;
        }

        private Task NavigateToPage<TPage>() => NavigateToPage<TPage>(parameter: null);

        private async Task NavigateToPage<TPage>(object parameter) {
            // Early out if already in the middle of a Navigation
            if(_isNavigating) {
                return;
            }

            _isNavigating = true;

            await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                Frame.Navigate(typeof(TPage), parameter: parameter);
            });
        }
    }
}
