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
            MenuItems.Add(SettingsMenuItem.Create("AdvancedInfo", "Advanced Info", "\uEE64", typeof(AdvancedInfoPage)));
            MenuItems.Add(SettingsMenuItem.Create("ColorProfile", "Color Profile", "\uE790", typeof(ColorProfile.MainPage)));
            MenuItems.Add(SettingsMenuItem.Create("GesturesAndTouch", "Gestures & Touch", "\uEDA4", typeof(GesturesTouch.MainPage)));
            MenuItems.Add(SettingsMenuItem.Create("GlanceScreen", "Glance Screen", "\uEE65", typeof(GlanceScreen.MainPage)));
            MenuItems.Add(SettingsMenuItem.Create("USBFunctionMode", "USB Function Mode", "\uECF0", typeof(USBFunctionMode.MainPage)));
            MenuItems.Add(SettingsMenuItem.Create("Vibration", "Vibration", "\uE877", typeof(Vibration.MainPage)));
            FooterMenuItems.Add(SettingsMenuItem.Create("About", "About", "\uE946", typeof(LumiaApp.Pages.AboutPage)));
        }

        private void NavView_SelectionChanged(MUXC.NavigationView sender, MUXC.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is SettingsMenuItem item && item.Page != SelectedSettings.CurrentSourcePageType)
            {
                SelectedSettings.Navigate(item.Page);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MenuItems.Count > 0)
                SelectedItem = MenuItems.First();
        }

        private void NavigationView_BackRequested(MUXC.NavigationView sender, MUXC.NavigationViewBackRequestedEventArgs args)
        {
            SelectedSettings.GoBack();
            SelectedItem = MenuItems.Where(x => x is SettingsMenuItem i && i.Page == SelectedSettings.CurrentSourcePageType).First();
        }
    }
}
