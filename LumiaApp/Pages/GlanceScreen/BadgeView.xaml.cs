using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GlanceScreen
{
    public sealed partial class BadgeView : UserControl
    {
        public BadgeView()
        {
            this.InitializeComponent();
            BadgeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));
            BadgeText.Text = "99+";
        }
    }
}
