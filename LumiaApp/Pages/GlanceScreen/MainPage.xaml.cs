using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GlanceScreen
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public SettingsModel ViewModel => LumiaApp.App.GlanceViewModel;

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;

            LockScreen.EffectiveViewportChanged += LockScreen_EffectiveViewportChanged;
            LockScreen.ChangeView(null, null, 0.4f);
        }

        private void LockScreen_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
        {
            LockScreen.ChangeView(null, null, 0.4f);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshEverything();
        }
    }
}
