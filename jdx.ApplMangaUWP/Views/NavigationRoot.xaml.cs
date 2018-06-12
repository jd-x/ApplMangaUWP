using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using jdx.ApplMangaUWP.Services;
using jdx.ApplMangaUWP.Services.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace jdx.ApplMangaUWP.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationRoot : Page {
        private static NavigationRoot _instance;
        private INavigationService _navigationService;
        private bool hasLoadedPreviously;

        public NavigationRoot() {
            _instance = this;
            this.InitializeComponent();

            var nav = SystemNavigationManager.GetForCurrentView();
            nav.BackRequested += NavView_BackRequested;
        }

        public static NavigationRoot Instance => _instance;

        public Frame AppFrame => AppNavFrame;

        public void InitializeNavigationService(INavigationService navigationService) {
            _navigationService = navigationService;
            // TODO: Hook into Navigation Events for loading screen
            _navigationService.Navigated += NavigationService_Navigated;
        }

        private void NavigationService_Navigated(object sender, EventArgs e) {
            var ignored = DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                var nav = SystemNavigationManager.GetForCurrentView();
                nav.AppViewBackButtonVisibility = _navigationService.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            ViewModelService.Instance.UnRegister();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            // TODO: Switch to Manga Reader if a manga is selected
        }

        private void NavView_BackRequested(object sender, BackRequestedEventArgs e) {
            var ignored = _navigationService.GoBackAsync();
            e.Handled = true;
        }

        private void AppNavFrame_Navigated(object sender, NavigationEventArgs e) {
            switch (e.SourcePageType) {
                case Type c when e.SourcePageType == typeof(Library):
                    ((NavigationViewItem)NavView.MenuItems[0]).IsSelected = true;
                    break;
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args) {
            if(args.IsSettingsInvoked) {
                Console.WriteLine("Setting invoked!");
                return;
            }

            switch(args.InvokedItem as string) {
                case "My library":
                    _navigationService.NavigateToLibraryAsync();
                    break;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            // Only do and initial navigate the first time the page loads
            // Switching out of CompactOverloadMode will fire this method but we don't
            // want to navigate because there is a page already loaded
            if(!hasLoadedPreviously) {
                _navigationService.NavigateToLibraryAsync();
                hasLoadedPreviously = true;
            }

            ViewModelService.Instance.Register(NavView, AppNavFrame);
        }
    }
}
