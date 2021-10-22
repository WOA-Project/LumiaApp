using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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
