using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace USBFunctionMode
{
    public class ModeDisplayItem
    {
        public string ConfigName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var items = USBFNController.Instance.GetListOfConfigurations()?.Select(
                x => new ModeDisplayItem()
                {
                    ConfigName = x,
                    Title = USBFNController.GetFunctionRoleFriendlyName(x),
                    Description = USBFNController.GetFunctionRoleDescription(x)
                }).OrderByDescending(x => x.Title.Split(" ")[0]);

            ModesListView.ItemsSource = items;
            ModesListView.SelectedIndex = items == null ? 0 : items.ToList().FindIndex(
                x => x.ConfigName.Equals(
                    USBFNController.Instance.GetCurrentConfiguration(), 
                    StringComparison.InvariantCultureIgnoreCase));
        }

        private void ModesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            USBFNController.Instance.SetCurrentConfiguration(((ModeDisplayItem)e.AddedItems[0]).ConfigName);
        }
    }
}
