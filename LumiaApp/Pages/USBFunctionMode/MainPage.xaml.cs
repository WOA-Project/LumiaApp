using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace USBFunctionMode
{
    public class ModeDisplayItem
    {
        public string ConfigName { get; }
        public string Title { get; }
        public string Description { get; }

        public ModeDisplayItem(string ConfigName, string Title, string Description)
        {
            this.ConfigName = ConfigName;
            this.Title = Title;
            this.Description = Description;
        }
    }

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            IOrderedEnumerable<ModeDisplayItem>? items = USBFNController.Instance.GetListOfConfigurations()?.Select(
                x => new ModeDisplayItem(x, USBFNController.GetFunctionRoleFriendlyName(x), USBFNController.GetFunctionRoleDescription(x))).OrderByDescending(x => x.Title.Split(" ")[0]);

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
