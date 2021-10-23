using System;
using System.ComponentModel;
using RegistryRT;

namespace GesturesTouch
{
    public class SettingsModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Registry registry => LumiaApp.App.Registry;

        private const string RegPath1 = @"SOFTWARE\OEM\Nokia\Touch\Improved";
        private const string RegPath2 = @"SOFTWARE\OEM\Nokia\Touch\WakeupGesture";

        private bool forbidWrites = true;

        public SettingsModel()
        {

        }

        public void RefreshEverything()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DoubleTapToWake"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GloveMode"));

            forbidWrites = false;
        }

        private bool SetRegistry(string RegPath, string ValueName, uint ValueData)
        {
            if (forbidWrites)
            {
                return false;
            }

            return registry.WriteValue(
                RegistryHive.HKEY_LOCAL_MACHINE,
                RegPath,
                ValueName,
                BitConverter.GetBytes(ValueData),
                RegistryType.Integer
            );
        }

        private bool GetRegistry(string RegPath, string ValueName, out uint ValueData)
        {
            ValueData = 0;
            byte[] buffer;

            bool status = registry.QueryValue(
                RegistryHive.HKEY_LOCAL_MACHINE,
                RegPath,
                ValueName,
                out RegistryType type,
                out buffer
            );

            if (status && type == RegistryType.Integer)
            {
                ValueData = BitConverter.ToUInt32(buffer, 0);
            }

            return status;
        }

        public bool DoubleTapToWake
        {
            get => GetDoubleTapToWake();
            set
            {
                if (value == DoubleTapToWake)
                    return;
                if (SetDoubleTapToWake(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DoubleTapToWake"));
            }
        }

        public bool GetDoubleTapToWake()
        {
            uint ValueData;
            GetRegistry(RegPath2, "Enabled", out ValueData);
            return ValueData == 1;
        }

        public bool SetDoubleTapToWake(bool value)
        {
            return SetRegistry(RegPath2, "Enabled", value ? 1u : 0u);
        }

        public bool GloveMode
        {
            get => GetGloveMode();
            set
            {
                if (value == GloveMode)
                    return;
                if (SetGloveMode(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GloveMode"));
            }
        }

        public bool GetGloveMode()
        {
            uint ValueData;
            GetRegistry(RegPath1, "TouchWithHighSensitivity", out ValueData);
            return ValueData == 1;
        }

        public bool SetGloveMode(bool value)
        {
            return SetRegistry(RegPath1, "TouchWithHighSensitivity", value ? 1u : 0u);
        }
    }
}
