using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Microsoft.UI.Xaml.Controls;
using muxc = Microsoft.UI.Xaml.Controls;

#if NETFX_CORE
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Windowing;
#endif

#nullable enable

namespace LumiaApp.Controls
{
    // https://docs.microsoft.com/windows/uwp/input-and-devices/designing-for-tv#custom-visual-state-trigger-for-xbox
    public class DeviceFamilyTrigger : StateTriggerBase
    {
        private string? _actualDeviceFamily;
        private string? _triggerDeviceFamily;

        public string DeviceFamily
        {
            get { return _triggerDeviceFamily ?? ""; }
            set
            {
                _triggerDeviceFamily = value;
                _actualDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
                SetActive(_triggerDeviceFamily == _actualDeviceFamily);
            }
        }
    }

    public enum DeviceType
    {
        Desktop,
        Mobile,
        Other,
        Xbox
    }

    [ContentProperty(Name = nameof(NavigationView))]
    public sealed partial class NavigationViewWindowControl : UserControl
    {
        public static readonly DependencyProperty NavigationViewProperty = DependencyProperty.Register(
          "NavigationView",
          typeof(muxc.NavigationView),
          typeof(NavigationViewWindowControl),
          new PropertyMetadata(null, OnNavigationViewPropertyChanged)
        );

        private static void OnNavigationViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavigationViewWindowControl ctrl)
            {
                ctrl.OnNavigationViewPropertyChanged(e);
            }
        }

        private void OnNavigationViewPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == NavigationViewProperty)
            {
                if (e.OldValue is muxc.NavigationView OldNavigationViewControl)
                {
                    DeinitializeNavigationViewHooks(OldNavigationViewControl);
                }

                if (e.NewValue is muxc.NavigationView NavigationViewControl)
                {
                    InitializeNavigationViewHooks(NavigationViewControl);
                }
            }
        }

        private void DeinitializeNavigationViewHooks(muxc.NavigationView NavigationViewControl)
        {
            NavigationViewControl.DisplayModeChanged -= NavigationViewControl_DisplayModeChanged;
            NavigationViewControl.Loaded -= OnNavigationViewControlLoaded;
            NavigationViewControl.PaneClosing -= NavigationViewControl_PaneClosing;
            NavigationViewControl.PaneOpening -= NavigationViewControl_PaneOpened;

            if (PaneDisplayModePropertyChangedToken != null)
                NavigationViewControl.UnregisterPropertyChangedCallback(muxc.NavigationView.PaneDisplayModeProperty, PaneDisplayModePropertyChangedToken.Value);
        }

        private long? PaneDisplayModePropertyChangedToken;

        private void InitializeNavigationViewHooks(muxc.NavigationView NavigationViewControl)
        {
            // Workaround for VisualState issue that should be fixed
            // by https://github.com/microsoft/microsoft-ui-xaml/pull/2271
            NavigationViewControl.PaneDisplayMode = muxc.NavigationViewPaneDisplayMode.Left;

            NavigationViewControl.IsTitleBarAutoPaddingEnabled = false;
            NavigationViewControl.IsTabStop = false;

            UpdateTitlebarVisibility(NavigationViewControl);
            UpdateNavigationViewDisplayMode(NavigationViewControl);

            if (NavigationViewControl.IsLoaded)
            {
                EnsureNavigationViewMatchNavigation();
            }

            NavigationViewControl.DisplayModeChanged += NavigationViewControl_DisplayModeChanged;
            NavigationViewControl.Loaded += OnNavigationViewControlLoaded;
            NavigationViewControl.PaneClosing += NavigationViewControl_PaneClosing;
            NavigationViewControl.PaneOpening += NavigationViewControl_PaneOpened;

            PaneDisplayModePropertyChangedToken = NavigationViewControl.RegisterPropertyChangedCallback(muxc.NavigationView.PaneDisplayModeProperty, new DependencyPropertyChangedCallback(OnPaneDisplayModeChanged));

            AppTitleBar.Height = NavigationViewControl.CompactPaneLength;
        }

        private void TitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (NavigationView != null)
            {
                UpdateTitlebarVisibility(NavigationView);
                UpdateNavigationViewDisplayMode(NavigationView);
            }
        }

        public muxc.NavigationView? NavigationView
        {
            get => (muxc.NavigationView?)GetValue(NavigationViewProperty);
            set => SetValue(NavigationViewProperty, value);
        }

#if !NETFX_CORE
        private static AppWindow GetAppWindowForCurrentWindow()
        {
            Window wnd = WindowManager.GetCurrentWindow().window;
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(wnd);
            Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(myWndId);
        }
#endif

        public void UpdateTitleBar(bool isLeftMode)
        {
#if NETFX_CORE
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
#else
            WindowManager.GetCurrentWindow().window.ExtendsContentIntoTitleBar = true;

            AppWindowTitleBar titleBar = TitleBar;
#endif

            if (NavigationView == null)
            {
                return;
            }

            if (isLeftMode)
            {
                NavigationView.PaneDisplayMode = muxc.NavigationViewPaneDisplayMode.Auto;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                NavigationView.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                NavigationView.PaneDisplayMode = muxc.NavigationViewPaneDisplayMode.Top;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                NavigationView.Margin = new Thickness(0, 48, 0, 0);
            }

            UpdateNavigationViewDisplayMode(NavigationView);
            UpdateAppTitleMargin(NavigationView);
        }

        public Action? NavigationViewLoaded { get; set; }

        public DeviceType DeviceFamily { get; set; }

#if NETFX_CORE
        private CoreApplicationViewTitleBar TitleBar { get; set; }
#else
        private AppWindowTitleBar TitleBar { get; set; }
#endif

        public NavigationViewWindowControl()
        {
            InitializeComponent();

            SetDeviceFamily();

#if NETFX_CORE
            Window.Current.SetTitleBar(AppTitleBar);

            TitleBar = CoreApplication.GetCurrentView().TitleBar;

            TitleBar.ExtendViewIntoTitleBar = true;

            TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
            TitleBar.IsVisibleChanged += TitleBar_IsVisibleChanged;
#else
            WindowManager.GetCurrentWindow().window.SetTitleBar(AppTitleBar);

            TitleBar = GetAppWindowForCurrentWindow().TitleBar;
            
            TitleBar.ExtendsContentIntoTitleBar = true;
#endif

            if (NavigationView != null)
            {
                InitializeNavigationViewHooks(NavigationView);
            }
        }

#if NETFX_CORE
        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateAppTitle(sender);
        }
#endif

        private void UpdateTitlebarVisibility(muxc.NavigationView navigationView)
        {
#if NETFX_CORE
            if (TitleBar.IsVisible)
            {
                Resources["NavigationViewContentMargin"] = new Thickness(0, 48, 0, 0);
            }
            else
            {
                Resources["NavigationViewContentMargin"] = new Thickness(0, 16, 0, 0);
            }

            AppTitleBar.Visibility = TitleBar.IsVisible ? Visibility.Visible : Visibility.Collapsed;
#endif
        }

        private void OnPaneDisplayModeChanged(DependencyObject sender, DependencyProperty dp)
        {
            var navigationView = (muxc.NavigationView)sender;
            UpdateTitlebarVisibility(navigationView);
        }

#if NETFX_CORE
        void UpdateAppTitle(CoreApplicationViewTitleBar coreTitleBar)
        {
            //ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }
#else
        void UpdateAppTitle(AppWindowTitleBar coreTitleBar)
        {
            //ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.RightInset, currMargin.Bottom);
        }
#endif

        public string GetAppTitleFromSystem()
        {
#if NETFX_CORE
            string AppTitle = (!string.IsNullOrEmpty(ApplicationView.GetForCurrentView().Title) ?
                ApplicationView.GetForCurrentView().Title + " - " : "") +
                "Lumia Settings";
#else
            string AppTitle = (!string.IsNullOrEmpty(GetAppWindowForCurrentWindow().Title) ?
                GetAppWindowForCurrentWindow().Title + " - " : "") +
                "Lumia Settings";
#endif

#if DEBUG
            AppTitle += " [DEBUG]";
#endif
            return AppTitle;
        }

        private void SetDeviceFamily()
        {
            var familyName = AnalyticsInfo.VersionInfo.DeviceFamily;

            if (!Enum.TryParse(familyName.Replace("Windows.", string.Empty), out DeviceType parsedDeviceType))
            {
                parsedDeviceType = DeviceType.Other;
            }

            DeviceFamily = parsedDeviceType;
        }

        private void OnNavigationViewControlLoaded(object sender, RoutedEventArgs e)
        {
            EnsureNavigationViewMatchNavigation();
        }

        private void EnsureNavigationViewMatchNavigation()
        {
            // Delay necessary to ensure NavigationView visual state can match navigation
            Task.Delay(500).ContinueWith(_ => this.NavigationViewLoaded?.Invoke(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void NavigationViewControl_PaneClosing(muxc.NavigationView sender, muxc.NavigationViewPaneClosingEventArgs args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavigationViewControl_PaneOpened(muxc.NavigationView sender, object args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavigationViewControl_DisplayModeChanged(muxc.NavigationView sender, muxc.NavigationViewDisplayModeChangedEventArgs args)
        {
            UpdateNavigationViewDisplayMode(sender);
        }

#if NETFX_CORE
        private void UpdateNavigationViewDisplayMode(muxc.NavigationView sender)
        {
            Thickness currMargin = AppTitleBar.Margin;
            Thickness navMargin = ((FrameworkElement)sender.Content).Margin;
            if (sender.PaneDisplayMode == muxc.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(TitleBar.SystemOverlayLeftInset, currMargin.Top, TitleBar.SystemOverlayRightInset, currMargin.Bottom);
                ((FrameworkElement)sender.Content).Margin = new Thickness(navMargin.Left, 0, navMargin.Right, navMargin.Bottom);
            }
            else if (sender.DisplayMode == muxc.NavigationViewDisplayMode.Minimal)
            {
                bool backVisible = (sender.IsBackButtonVisible == muxc.NavigationViewBackButtonVisible.Auto && AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Xbox") || sender.IsBackButtonVisible == muxc.NavigationViewBackButtonVisible.Visible;
                AppTitleBar.Margin = new Thickness(TitleBar.SystemOverlayLeftInset + (backVisible ? sender.CompactPaneLength * 2 : sender.CompactPaneLength), currMargin.Top, TitleBar.SystemOverlayRightInset, currMargin.Bottom);
                ((FrameworkElement)sender.Content).Margin = new Thickness(navMargin.Left, TitleBar.IsVisible ? sender.CompactPaneLength : 0, navMargin.Right, navMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(TitleBar.SystemOverlayLeftInset + sender.CompactPaneLength, currMargin.Top, TitleBar.SystemOverlayRightInset, currMargin.Bottom);
                ((FrameworkElement)sender.Content).Margin = new Thickness(navMargin.Left, 0, navMargin.Right, navMargin.Bottom);
            }

            UpdateAppTitleMargin(sender);
        }

#else
        private void UpdateNavigationViewDisplayMode(muxc.NavigationView sender)
        {
            Thickness currMargin = AppTitleBar.Margin;
            Thickness navMargin = ((FrameworkElement)sender.Content).Margin;
            if (sender.PaneDisplayMode == muxc.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(GetAppWindowForCurrentWindow().TitleBar.LeftInset, currMargin.Top, GetAppWindowForCurrentWindow().TitleBar.RightInset, currMargin.Bottom);
                ((FrameworkElement)sender.Content).Margin = new Thickness(navMargin.Left, 0, navMargin.Right, navMargin.Bottom);
            }
            else if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal && sender.PaneDisplayMode != muxc.NavigationViewPaneDisplayMode.Top)
            {
                bool backVisible = (sender.IsBackButtonVisible == muxc.NavigationViewBackButtonVisible.Auto && AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Xbox") || sender.IsBackButtonVisible == muxc.NavigationViewBackButtonVisible.Visible;
                AppTitleBar.Margin = new Thickness(GetAppWindowForCurrentWindow().TitleBar.LeftInset + (backVisible ? sender.CompactPaneLength * 2 : sender.CompactPaneLength), currMargin.Top, GetAppWindowForCurrentWindow().TitleBar.RightInset, currMargin.Bottom);
                ((FrameworkElement)sender.Content).Margin = new Thickness(navMargin.Left, sender.CompactPaneLength, navMargin.Right, navMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(GetAppWindowForCurrentWindow().TitleBar.LeftInset + sender.CompactPaneLength, currMargin.Top, GetAppWindowForCurrentWindow().TitleBar.RightInset, currMargin.Bottom);
                ((FrameworkElement)sender.Content).Margin = new Thickness(navMargin.Left, 0, navMargin.Right, navMargin.Bottom);
            }

            UpdateAppTitleMargin(sender);
        }
#endif

        private void UpdateAppTitleMargin(muxc.NavigationView sender)
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24, topLeftIndent = 12;

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                AppTitleAndIcon.TranslationTransition = new Vector3Transition();

                if (sender.PaneDisplayMode == muxc.NavigationViewPaneDisplayMode.Top)
                {
                    AppTitleAndIcon.Translation = new System.Numerics.Vector3(topLeftIndent, 0, 0);
                }
                else if ((sender.DisplayMode == muxc.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == muxc.NavigationViewDisplayMode.Minimal)
                {
                    AppTitleAndIcon.Translation = new System.Numerics.Vector3(smallLeftIndent, 0, 0);
                }
                else
                {
                    AppTitleAndIcon.Translation = new System.Numerics.Vector3(largeLeftIndent, 0, 0);
                }
            }
            else
            {
                Thickness currMargin = AppTitleAndIcon.Margin;

                if (sender.PaneDisplayMode == muxc.NavigationViewPaneDisplayMode.Top)
                {
                    AppTitleAndIcon.Translation = new System.Numerics.Vector3(topLeftIndent, 0, 0);
                }
                else if ((sender.DisplayMode == muxc.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == muxc.NavigationViewDisplayMode.Minimal)
                {
                    AppTitleAndIcon.Margin = new Thickness(smallLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
                else
                {
                    AppTitleAndIcon.Margin = new Thickness(largeLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
            }
        }

        private void ColumnView_Loaded(object sender, RoutedEventArgs e)
        {
#if NETFX_CORE
            UpdateAppTitle(CoreApplication.GetCurrentView().TitleBar);
#else
            UpdateAppTitle(TitleBar);
#endif
            // remove the solid-colored backgrounds behind the caption controls and system back button if we are in left mode
            // This is done when the app is loaded since before that the actual theme that is used is not "determined" yet
            UpdateTitleBar(true);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).Parent != null)
            {
                return;
            }

            if (NavigationView != null)
            {
                DeinitializeNavigationViewHooks(NavigationView);
            }

#if NETFX_CORE
            TitleBar.LayoutMetricsChanged -= TitleBar_LayoutMetricsChanged;
            TitleBar.IsVisibleChanged -= TitleBar_IsVisibleChanged;
#endif
        }
    }
}
