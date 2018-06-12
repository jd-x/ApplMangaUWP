using System;
using System.ComponentModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace jdx.ApplMangaUWP.Helpers {
    public class TitleBarHelper : INotifyPropertyChanged {
        #region Private variables

        private static TitleBarHelper _instance = new TitleBarHelper();
        private static CoreApplicationViewTitleBar _coreTitleBar;
        private Thickness _titlePosition;
        private Visibility _titleVisibility;

        #endregion

        #region Public properties

        public static TitleBarHelper Instance => _instance;

        public CoreApplicationViewTitleBar TitleBar => _coreTitleBar;

        public Thickness TitlePosition {
            get => _titlePosition;

            set {
                if (value.Left != _titlePosition.Left || value.Top != _titlePosition.Top) {
                    _titlePosition = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitlePosition)));
                }
            }
        }

        public Visibility TitleVisibility {
            get => _titleVisibility;

            set {
                _titleVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitleVisibility)));
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleBarHelper"/> class.
        /// </summary>
        public TitleBarHelper() {
            _coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            _coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            _titlePosition = CalculateTitleBarOffset(_coreTitleBar.SystemOverlayLeftInset, _coreTitleBar.Height);
            _titleVisibility = Visibility.Visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void GoFullScreen() => TitleVisibility = Visibility.Collapsed;

        public void ExitFullScreen() => TitleVisibility = Visibility.Visible;

        private Thickness CalculateTitleBarOffset(double systemOverlayLeftInset, double height) {
            // Top position should be 6px for a 32px high title bar hence scale by actual height
            var correctHeight = height / 32 * 6;

            return new Thickness(systemOverlayLeftInset + 12, correctHeight, 0, 0);
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args) {
            TitlePosition = CalculateTitleBarOffset(_coreTitleBar.SystemOverlayLeftInset, _coreTitleBar.Height);
        }
    }
}
