using System;
using System.Linq;
using Windows.UI.Xaml;

#nullable enable

namespace LumiaApp.Controls.Settings
{
    public class SettingsThemeResources : ResourceDictionary
    {
        public SettingsThemeResources()
        {
            MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("ms-appx:///Themes/Settings.xaml") });
            MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("ms-appx:///Styles/TextBlock.xaml") });
            MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("ms-appx:///Styles/Button.xaml") });
        }
    }
}
