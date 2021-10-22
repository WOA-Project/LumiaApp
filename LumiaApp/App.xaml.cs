using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using RegistryRT;
using AdvancedInfoRT;

namespace LumiaApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Registry Registry { get; } = new Registry();
        public static AIRT AIRT { get; } = new AIRT();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            Registry.InitNTDLLEntryPoints();
            AIRT.InitNTDLLEntryPoints();
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
