using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdvancedInfo.ContentDialogs
{
    public sealed partial class SomethingHappened : ContentDialog
    {
        public SomethingHappened(string Error)
        {
            this.InitializeComponent();
            CrashText.Text = Error;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
