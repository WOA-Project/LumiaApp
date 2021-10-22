using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RegistryRT;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Vibration
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Registry regrt;

        bool ran1 = false;
        bool ran2 = false;

        public MainPage()
        {
            regrt = new Registry();
            regrt.InitNTDLLEntryPoints();

            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var result = ReadRegistryDwordFromVibra("Enabled");
            if (result != -1)
                EnableToggle.IsOn = result == 1;

            result = ReadRegistryDwordFromVibra("Intensity");
            if (result != -1 && result >= 20 && result <= 100)
                IntensitySlider.Value = result;

            result = ReadRegistryDwordFromVibra("Duration");
            if (result != -1 && result >= 1 && result <= 10)
                DurationSlider.Value = result;
        }

        private int ReadRegistryDwordFromVibra(string Value)
        {
            return ReadRegistryDword(RegistryHive.HKEY_LOCAL_MACHINE, "SOFTWARE\\OEM\\Vibra", Value);
        }

        private void SetRegistryDwordFromVibra(string Value, int Data)
        {
            regrt.WriteValue(RegistryHive.HKEY_LOCAL_MACHINE, "SOFTWARE\\OEM\\Vibra", Value, BitConverter.GetBytes(Data), 4);
        }

        private int ReadRegistryDword(RegistryHive hive, string Key, string Value)
        {
            uint regtype;
            byte[] buffer;
            bool result = regrt.QueryValue(hive, Key, Value, out regtype, out buffer);
            if (result)
            {
                return BitConverter.ToInt32(buffer, 0);
            }
            return -1;
        }

        private void EnableToggle_Toggled(object sender, RoutedEventArgs e)
        {
            SetRegistryDwordFromVibra("Enabled", (sender as ToggleSwitch).IsOn ? 1 : 0);

            if ((sender as ToggleSwitch).IsOn)
                SendHapticFeedback();
        }

        private void DurationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!ran1)
                ran1 = true;
            else
                SetRegistryDwordFromVibra("Duration", (int)(sender as Slider).Value);

            SendHapticFeedback();
        }

        private async void SendHapticFeedback()
        {
            try
            {
                var vibration = await Windows.Devices.Haptics.VibrationDevice.RequestAccessAsync();
                if (vibration == Windows.Devices.Haptics.VibrationAccessStatus.Allowed)
                {
                    var vibrationdevice = await Windows.Devices.Haptics.VibrationDevice.GetDefaultAsync();
                    if (vibrationdevice != null)
                    {
                        var feedback = vibrationdevice.SimpleHapticsController.SupportedFeedback.First();

                        vibrationdevice.SimpleHapticsController.SendHapticFeedbackForDuration(feedback, IntensitySlider.Value / 100d, new TimeSpan((long)DurationSlider.Value * 1000000L));
                    }
                }
            }
            catch
            {

            }
        }

        private void IntensitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!ran2)
                ran2 = true;
            else
                SetRegistryDwordFromVibra("Intensity", (int)(sender as Slider).Value);

            SendHapticFeedback();
        }
    }
}
