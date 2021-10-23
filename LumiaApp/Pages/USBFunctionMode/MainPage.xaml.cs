using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

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

            IOrderedEnumerable<ModeDisplayItem> items = USBFNController.Instance.GetListOfConfigurations()?.Select(
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
