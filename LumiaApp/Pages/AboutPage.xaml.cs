using Microsoft.Toolkit.Uwp.Helpers;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace LumiaApp.Pages
{
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
            VersionTextBlock.Text = Package.Current.Id.Version.ToFormattedString();
        }
    }
}
