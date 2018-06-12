using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jdx.ApplMangaUWP.Helpers;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace jdx.ApplMangaUWP.Services {
    public class ViewModelService {
        // Compact mode
        private static readonly ViewModelService _instance = new ViewModelService();
        private UIElement _previousViewContent;

        // Fullscreen
        private double oldCompactThreshold;
        private double oldExpandedThreshold;
        private Brush oldBackgroundBrush;
        private FrameworkElement _navHeaeder;
        private FrameworkElement _menuButton;
        private NavigationView _navigationView;
        private Frame _appNavFrame;
        private bool inFullScreen;

        public static ViewModelService Instance => _instance;

        public async Task CreateNewView<T>(Func<T> newViewObjectFactory, Action<T> loadAction)
            where T : UIElement {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            var newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                var newViewContent = newViewObjectFactory();
                Window.Current.Content = newViewContent;
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;

                loadAction(newViewContent);
            });

            var viewShown = await ApplicationViewSwitcher.TryShowAsViewModeAsync(newViewId, ApplicationViewMode.CompactOverlay);
        }

        public async Task<bool> SwitchToCompactOverlay<T>(T newView, Action<T> loadAction)
            where T : UIElement {
            _previousViewContent = Window.Current.Content;
            bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
            if(modeSwitched) {
                Window.Current.Content = _previousViewContent;
                _previousViewContent = null;
            }

            return modeSwitched;
        }

        public async Task<bool> SwitchToNormalMode() {
            bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
            if(modeSwitched) {
                Window.Current.Content = _previousViewContent;
                _previousViewContent = null;
            }

            return modeSwitched;
        }

        public void DoEnterFullscreen() {
            var view = ApplicationView.GetForCurrentView();
            inFullScreen = view.TryEnterFullScreenMode();
            if(inFullScreen) {
                // Adjust NavigationView to accommodate full screen
                if(_navigationView != null) {
                    oldBackgroundBrush = _navigationView.Background;

                    CollapseNavigationViewToBurger();

                    EnsureElements(_navigationView);

                    // Hide NavigationView menu while in full screen
                    _menuButton.Visibility = Visibility.Collapsed;
                }

                // Hide title bar
                TitleBarHelper.Instance.GoFullScreen();

                // If current page is interested in full screen, tell it
                if(_appNavFrame != null && _appNavFrame.Content is IFullScreenPage) {
                    ((IFullScreenPage)_appNavFrame.Content).EnterFullScreen();
                }
            }
        }

        private void EnsureElements(NavigationView navigationView) {
            if (_navHeaeder == null) {
                _navHeaeder = VisualHelpers.GetVisualChildByName<FrameworkElement>(navigationView, "HeaderContent");
            }

            if(_menuButton == null) {
                _menuButton = VisualHelpers.GetVisualChildByName<FrameworkElement>(navigationView, "TogglePaneButton");
            }
        }

        private void CollapseNavigationViewToBurger() {
            EnsureElements(_navigationView);

            oldCompactThreshold = _navigationView.CompactModeThresholdWidth;
            oldExpandedThreshold = _navigationView.ExpandedModeThresholdWidth;

            // Force NavView to collapse to its narrowest mode
            _navigationView.CompactModeThresholdWidth = 10000;
            _navigationView.ExpandedModeThresholdWidth = 10000;

            // Collapse NavigationView header while in full screen
            _navHeaeder.Visibility = Visibility.Collapsed;
        }

        public void RestoreNavigationViewDefault() {
            _navigationView.CompactModeThresholdWidth = oldCompactThreshold;
            _navigationView.ExpandedModeThresholdWidth = oldExpandedThreshold;
            _navHeaeder.Visibility = Visibility.Visible;
        }

        public void DoExitFullScreen() {
            if(inFullScreen) {
                var view = ApplicationView.GetForCurrentView();
                view.ExitFullScreenMode();

                TitleBarHelper.Instance.ExitFullScreen();

                if(_navigationView != null) {
                    EnsureElements(_navigationView);
                    RestoreNavigationViewDefault();

                    _navigationView.Background = oldBackgroundBrush;

                    _menuButton.Visibility = Visibility.Visible;
                }

                if (_appNavFrame != null && _appNavFrame.Content is IFullScreenPage) {
                    ((IFullScreenPage)_appNavFrame.Content).ExitFullScreen();
                }

                inFullScreen = false;
            }
        }

        public void ToggleFullScreen() {

        }

        public void Register(NavigationView navView, Frame appNavName) {
            _navigationView = navView;
            _appNavFrame = appNavName;
        }

        public void UnRegister() {
            _navigationView = null;
            _appNavFrame = null;
        }
    }
}
