using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AdvancedInfo.Handlers;
using Windows.Storage;
using System.Linq;
using Windows.UI.Xaml.Input;
using Windows.System;

namespace LumiaApp
{
    public sealed partial class AdvancedInfoPage : Page
    {
        private RegistryHandler reghandler;
        private DPPHandler dpphandler;
        private ModemHandler modemhandler;
        private InternalHandler internalhandler;

        public AdvancedInfoPage()
        {
            this.InitializeComponent();

            RetrieveData();
        }

        public bool IsDarkTheme { get { return (bool)Application.Current.Resources["IsDarkTheme"]; } }

        private async void RetrieveData()
        {
            try
            {
                internalhandler = new InternalHandler();
                RAM.Text = internalhandler.RAM;
                Build.Text = internalhandler.FirmwareBuild;
            }
            catch
            {

            }

            try
            {
                modemhandler = await ModemHandler.LoadHandlerAsync();
                PhoneListView.ItemsSource = modemhandler.ModemInformation;
            }
            catch
            {

            }

            try
            {
                reghandler = new RegistryHandler();
                OEM.Text = reghandler.ProductCode;
                MO.Text = reghandler.MobileOperator;
                SP.Text = reghandler.ServiceProvider;
                CSV.Text = reghandler.SOC;
                TitleBlock.Text = reghandler.Manufacturer + " " + reghandler.ModelName;
                SubtitleBlock.Text = reghandler.ProductCodeCleaned;

                if (reghandler.ReleaseName != null)
                {
                    VersionNameText.Text = VersionNameText.Text.Replace("XXXX", reghandler.ReleaseName);
                }
            }
            catch
            {

            }

            try
            {
                dpphandler = await DPPHandler.LoadHandlerAsync();

                IMEI.Text = dpphandler.IMEI;

                foreach (string Line in dpphandler.Product.Split('\n'))
                {
                    if (Line.StartsWith("HWID"))
                    {
                        HR.Text = string.Join(".", Line.Split(":").Last().ToCharArray());
                    }
                    else if (Line.StartsWith("CTR"))
                    {
                        PC.Text = Line.Split(":").Last();
                    }
                }

                ManufactureCountry.Text = dpphandler.COO ?? "N/A";
                RegulatoryImage.Source = IsDarkTheme ? dpphandler.RegulatoryBlack : dpphandler.RegulatoryWhite;
                RegulatoryPanel.Visibility = Visibility.Visible;
            }
            catch
            {
                FileSystemAccessWarning.IsOpen = true;
            }

            StorageFolder local = ApplicationData.Current.LocalFolder;
            System.Collections.Generic.IDictionary<string, object> retrivedProperties = await local.Properties.RetrievePropertiesAsync(new string[] { "System.FreeSpace" });
            ulong freeSpace = (UInt64)retrivedProperties["System.FreeSpace"];
            retrivedProperties = await local.Properties.RetrievePropertiesAsync(new string[] { "System.Capacity" });
            ulong totalSpace = (UInt64)retrivedProperties["System.Capacity"];

            ulong usedSpace = totalSpace - freeSpace;

            InternalStorageUsage.Maximum = totalSpace;
            InternalStorageUsage.Value = usedSpace;

            UsageDesc.Text = FormatBytes(freeSpace) + " free out of " + FormatBytes(totalSpace);
        }

        private static string FormatBytes(ulong bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        private async void ReleasePanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://woa-project.github.io/LumiaWOA"));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-broadfilesystemaccess"));
        }
    }
}
