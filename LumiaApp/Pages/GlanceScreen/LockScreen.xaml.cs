using lockframework;
using System;
using Windows.ApplicationModel.LockScreen;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

#nullable enable

namespace GlanceScreen
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LockScreen : UserControl
    {
        public SettingsModel ViewModel => LumiaApp.App.GlanceViewModel;

        public LockScreenInfo? LockScreenInfo;

        public string TimeStr;
        public string DateStr;
        public BitmapImage? BgImage;

        public LockScreen()
        {
            this.InitializeComponent();

            TimeStr = DateTime.Now.ToString("t");
            DateStr = DateTime.Now.ToString("D").Replace(", " + DateTime.Now.Year.ToString(), "").Replace(" " + DateTime.Now.Year.ToString(), "").Replace(DateTime.Now.Year.ToString(), "");

            try
            {
                LockScreenInfo = LockAppBroker.CreateLockScreenInfo();

                BgImage = new BitmapImage();
                BgImage.SetSource(LockScreenInfo.LockScreenImage);
            }
            catch { }
        }
    }
}
