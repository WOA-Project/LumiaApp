using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using AdvancedInfo.Handlers;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Hosting;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using System.Numerics;
using Windows.UI.Composition;
using AdvancedInfo.Extensions;
using Windows.System;
using AdvancedInfo.ContentDialogs;
using Windows.Storage;
using System.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdvancedInfo.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainInformationPage : Page
    {
        RegistryHandler reghandler;
        DPPHandler dpphandler;
        ModemHandler modemhandler;
        InternalHandler internalhandler;

        CompositionPropertySet _props;
        CompositionPropertySet _scrollerPropertySet;
        Compositor _compositor;
        Visual headerVisual;

        public MainInformationPage()
        {
            this.InitializeComponent();

            RetrieveData();

            Loaded += MainInformationPage_Loaded;
            SizeChanged += MainInformationPage_SizeChanged;
        }
        private void MainInformationPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (headerVisual != null)
            {
                //Set the header's CenterPoint to ensure the overpan scale looks as desired
                headerVisual.CenterPoint = new Vector3((float)(Header.ActualWidth / 2), (float)Header.ActualHeight, 0);
            }
        }

        private void MainInformationPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Update the ZIndex of the header container so that the header is above the items when scrolling
            Canvas.SetZIndex(Header, 1);

            // Get the PropertySet that contains the scroll values from the ScrollViewer
            _scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
            _compositor = _scrollerPropertySet.Compositor;

            // Create a PropertySet that has values to be referenced in the ExpressionAnimations below
            _props = _compositor.CreatePropertySet();
            _props.InsertScalar("progress", 0);
            _props.InsertScalar("clampSize", 150);
            _props.InsertScalar("scaleFactor", 0.7f);

            // Get references to our property sets for use with ExpressionNodes
            var scrollingProperties = _scrollerPropertySet.GetSpecializedReference<ManipulationPropertySetReferenceNode>();
            var props = _props.GetReference();
            var progressNode = props.GetScalarProperty("progress");
            var clampSizeNode = props.GetScalarProperty("clampSize");

            // Create and start an ExpressionAnimation to track scroll progress over the desired distance
            ExpressionNode progressAnimation = ExpressionFunctions.Clamp(-scrollingProperties.Translation.Y / clampSizeNode, 0, 1);
            _props.StartAnimation("progress", progressAnimation);

            // Get the backing visual for the header so that its properties can be animated
            headerVisual = ElementCompositionPreview.GetElementVisual(Header);

            // Create and start an ExpressionAnimation to clamp the header's offset to keep it onscreen
            ExpressionNode headerTranslationAnimation = ExpressionFunctions.Conditional(progressNode < 1, 0, -scrollingProperties.Translation.Y - clampSizeNode);
            headerVisual.StartAnimation("Offset.Y", headerTranslationAnimation);

            // Create and start an ExpressionAnimation to scale the header during overpan
            ExpressionNode headerScaleAnimation = ExpressionFunctions.Lerp(1, 1.25f, ExpressionFunctions.Clamp(scrollingProperties.Translation.Y / 50, 0, 1));
            headerVisual.StartAnimation("Scale.X", headerScaleAnimation);
            headerVisual.StartAnimation("Scale.Y", headerScaleAnimation);

            //Set the header's CenterPoint to ensure the overpan scale looks as desired
            headerVisual.CenterPoint = new Vector3((float)(Header.ActualWidth / 2), (float)Header.ActualHeight, 0);

            ExpressionNode OpacityAnimation = ExpressionFunctions.Clamp(1 - (progressNode * 2), 0, 1);

            Visual profileVisual = ElementCompositionPreview.GetElementVisual(ProfileImage);
            profileVisual.StartAnimation("Opacity", OpacityAnimation);

            Visual subtitleVisual = ElementCompositionPreview.GetElementVisual(SubtitleBlock);
            subtitleVisual.StartAnimation("Opacity", OpacityAnimation);

            // Get the backing visuals for the text and button containers so that their properites can be animated
            Visual buttonVisual = ElementCompositionPreview.GetElementVisual(ButtonPanel);

            ExpressionNode buttonOffsetAnimation = progressNode * -14;
            buttonVisual.StartAnimation("Offset.Y", buttonOffsetAnimation);
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
                ManufactureCountry.Text = dpphandler.COO;
                RegulatoryImage.Source = IsDarkTheme ? dpphandler.RegulatoryBlack : dpphandler.RegulatoryWhite;

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
            }
            catch
            {
                await new NotEnoughRights().ShowAsync();
            }

            StorageFolder local = ApplicationData.Current.LocalFolder;
            var retrivedProperties = await local.Properties.RetrievePropertiesAsync(new string[] { "System.FreeSpace" });
            var freeSpace = (UInt64)retrivedProperties["System.FreeSpace"];
            retrivedProperties = await local.Properties.RetrievePropertiesAsync(new string[] { "System.Capacity" });
            var totalSpace = (UInt64)retrivedProperties["System.Capacity"];

            var usedSpace = totalSpace - freeSpace;

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

        private async void GetSupport_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://go.microsoft.com/fwlink/?LinkId=519171"));
        }

        private async void GetAccessories_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://go.microsoft.com/fwlink/?LinkId=526929"));
        }

        private void More_Click(object sender, RoutedEventArgs e)
        {
            var flyout = new MenuFlyout();
            var item = new MenuFlyoutItem() { Text = "View Windows specific information" };
            item.Click += async (object sender2, RoutedEventArgs e2) =>
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:about"));
            };
            flyout.Items.Add(item);
            flyout.ShowAt(sender as FrameworkElement);
        }

        private async void ReleasePanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://woaproject.net"));
        }
    }
}
