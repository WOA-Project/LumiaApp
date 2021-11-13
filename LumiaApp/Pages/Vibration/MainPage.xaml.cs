using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using RegistryRT;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Vibration
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Registry regrt => LumiaApp.App.Registry;

        bool ran1 = false;
        bool ran2 = false;

        public MainPage()
        {
            this.InitializeComponent();

            int result = ReadRegistryDwordFromVibra("Enabled");
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
            SetRegistryDwordFromVibra("Enabled", ((ToggleSwitch)sender).IsOn ? 1 : 0);

            if (((ToggleSwitch)sender).IsOn)
                SendHapticFeedback();
        }

        private void DurationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!ran1)
                ran1 = true;
            else
                SetRegistryDwordFromVibra("Duration", (int)((Slider)sender).Value);

            SendHapticFeedback();
        }

        private async void SendHapticFeedback()
        {
            try
            {
                Windows.Devices.Haptics.VibrationAccessStatus vibration = await Windows.Devices.Haptics.VibrationDevice.RequestAccessAsync();
                if (vibration == Windows.Devices.Haptics.VibrationAccessStatus.Allowed)
                {
                    Windows.Devices.Haptics.VibrationDevice vibrationdevice = await Windows.Devices.Haptics.VibrationDevice.GetDefaultAsync();
                    if (vibrationdevice != null)
                    {
                        Windows.Devices.Haptics.SimpleHapticsControllerFeedback feedback = vibrationdevice.SimpleHapticsController.SupportedFeedback.First();

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
                SetRegistryDwordFromVibra("Intensity", (int)((Slider)sender).Value);

            SendHapticFeedback();
        }
    }
}
