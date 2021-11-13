using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AdvancedInfo.Handlers;

namespace LumiaApp
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryHandler reghandler = new();
                TitleBlock.Text = $"{reghandler.Manufacturer} {reghandler.ModelName}";
                SubtitleBlock.Text = reghandler.ProductCodeCleaned;
            }
            catch
            {

            }
        }
    }
}
