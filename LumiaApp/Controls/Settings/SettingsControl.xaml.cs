using System;
using System.Linq;
using Windows.UI.Xaml;
using MUXC = Microsoft.UI.Xaml.Controls;

namespace LumiaApp.Controls.Settings
{
    public class SettingsMenuItem
    {
        public string Key { get; }
        public string Title { get; }
        public string Icon { get; }
        public Type Page { get; }

        public SettingsMenuItem(string Key, string Title, string Icon, Type Page)
        {
            this.Key = Key;
            this.Title = Title;
            this.Icon = Icon;
            this.Page = Page;
        }

        public static SettingsMenuItem Create(string Key, string Title, string Icon, Type Page)
        {
            return new(Key, Title, Icon, Page);
        }
    }

    public sealed partial class SettingsControl : MUXC.NavigationView
    {
        public SettingsControl()
        {
            this.InitializeComponent();
            // TODO: Update pages
            MenuItems.Add(SettingsMenuItem.Create("AdvancedInfo", "Advanced Info", "\uE90D", typeof(AdvancedInfo.Pages.MainInformationPage)));
            MenuItems.Add(SettingsMenuItem.Create("ColorProfile", "Color Profile", "\uE90D", typeof(ColorProfile.MainPage)));
            MenuItems.Add(SettingsMenuItem.Create("GesturesAndTouch", "Gestures & Touch", "\uE90D", typeof(GesturesTouch.MainPage)));
            MenuItems.Add(SettingsMenuItem.Create("GlanceScreen", "Glance Screen", "\uE90D", typeof(GlanceScreen.MainPage)));
            MenuItems.Add(SettingsMenuItem.Create("USBFunctionMode", "USB Function Mode", "\uE90D", typeof(USBFunctionMode.MainPage)));
            MenuItems.Add(SettingsMenuItem.Create("Vibration", "Vibration", "\uE90D", typeof(Vibration.MainPage)));
            FooterMenuItems.Add(SettingsMenuItem.Create("About", "About", "\uE90D", typeof(LumiaApp.Pages.AboutPage)));
        }

        private void NavView_SelectionChanged(MUXC.NavigationView sender, MUXC.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is SettingsMenuItem item)
            {
                SelectedSettings.Navigate(item.Page);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MenuItems.Count > 0)
                SelectedItem = MenuItems.First();
        }
    }
}
