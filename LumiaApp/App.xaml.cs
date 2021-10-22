using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using RegistryRT;
using AdvancedInfoRT;
using Windows.UI.Popups;
using System;

namespace LumiaApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Registry Registry { get; } = new Registry();
        public static AIRT AIRT { get; } = new AIRT();
        public static GlanceScreen.SettingsModel GlanceViewModel { get; } = new GlanceScreen.SettingsModel();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += App_UnhandledException;
            Registry.InitNTDLLEntryPoints();
            AIRT.InitNTDLLEntryPoints();
        }

        private async void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog(e.Exception.StackTrace, e.Exception.Message).ShowAsync();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (Window.Current.Content is not SettingsPage)
            {
                Window.Current.Content = new SettingsPage();
            }

            if (!e.PrelaunchActivated)
            {
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }
    }
}
