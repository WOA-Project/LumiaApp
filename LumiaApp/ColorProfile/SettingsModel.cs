using RegistryRT;
using System;
using System.ComponentModel;

namespace ColorProfile
{
    public enum ProfileMode
    {
        Standard = 0,
        Vivid = 1,
        Cool = 2,
        Advanced = 3,
        NightLight = 4
    }

    public class SettingsModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Registry registry = new Registry();

        private const string RegPath = @"SOFTWARE\OEM\Nokia\Display\ColorAndLight";

        private bool forbidWrites = true;

        public SettingsModel()
        {
            registry.InitNTDLLEntryPoints();
        }

        public void RefreshEverything()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BatterySaverBrightness"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SunlightReadability"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Profile"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Temperature"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tint"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Saturation"));

            forbidWrites = false;
        }

        private bool SetRegistrySZ(string ValueName, string ValueData)
        {
            if (forbidWrites)
            {
                return false;
            }

            return registry.WriteValue(
                RegistryHive.HKEY_LOCAL_MACHINE,
                RegPath,
                ValueName,
                System.Text.Encoding.Unicode.GetBytes(ValueData + "\0"),
                RegistryType.String
            );
        }

        private bool GetRegistrySZ(string ValueName, out string ValueData)
        {
            ValueData = "";
            byte[] buffer;

            bool status = registry.QueryValue(
                RegistryHive.HKEY_LOCAL_MACHINE,
                RegPath,
                ValueName,
                out RegistryType type,
                out buffer
            );

            if (status && type == RegistryType.String && buffer != null)
            {
                ValueData = System.Text.Encoding.Unicode.GetString(buffer, 0, buffer.Length - 2);
            }

            return status;
        }

        private bool SetRegistry(string ValueName, uint ValueData)
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

        private bool GetRegistry(string ValueName, out uint ValueData)
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

        private string ConvertProfileMode(ProfileMode ProfileMode)
        {
            switch (ProfileMode)
            {
                case ProfileMode.Standard:
                    return "Standard.icm";
                case ProfileMode.Vivid:
                    return "Vivid.icm";
                case ProfileMode.Cool:
                    return "Cool.icm";
                case ProfileMode.Advanced:
                    return "Advanced.icm";
                case ProfileMode.NightLight:
                    return "Night light.icm";
                default:
                    return "Standard.icm";
            }
        }

        private ProfileMode ConvertProfileMode(string ProfileModeSZ)
        {
            switch (ProfileModeSZ)
            {
                case "Standard.icm":
                    return ProfileMode.Standard;
                case "Vivid.icm":
                    return ProfileMode.Vivid;
                case "Cool.icm":
                    return ProfileMode.Cool;
                case "Advanced.icm":
                    return ProfileMode.Advanced;
                case "Night light.icm":
                    return ProfileMode.NightLight;
                default:
                    return ProfileMode.Standard;
            }
        }

        public bool BatterySaverBrightness
        {
            get => GetBatterySaverBrightness();
            set
            {
                if (value == BatterySaverBrightness)
                    return;
                if (SetBatterySaverBrightness(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BatterySaverBrightness"));
            }
        }

        public bool GetBatterySaverBrightness()
        {
            uint ValueData;
            GetRegistry("UserSettingBsmDimmingEnabled", out ValueData);
            return ValueData == 1;
        }

        public bool SetBatterySaverBrightness(bool value)
        {
            return SetRegistry("UserSettingBsmDimmingEnabled", value ? 1u : 0u);
        }

        public bool SunlightReadability
        {
            get => GetSunlightReadability();
            set
            {
                if (value == SunlightReadability)
                    return;
                if (SetSunlightReadability(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SunlightReadability"));
            }
        }

        public bool GetSunlightReadability()
        {
            uint ValueData;
            GetRegistry("UserSettingSreEnabled", out ValueData);
            return ValueData == 1;
        }

        public bool SetSunlightReadability(bool value)
        {
            return SetRegistry("UserSettingSreEnabled", value ? 1u : 0u);
        }

        public int Profile
        {
            get => (int)GetProfile();
            set
            {
                if (value == Profile)
                    return;
                if (SetProfile((ProfileMode)value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Profile"));
            }
        }

        private bool SetProfile(ProfileMode profile)
        {
            switch (profile)
            {
                case ProfileMode.Vivid:
                    Profiles.Vivid.ApplyProfile();
                    break;
                case ProfileMode.Cool:
                    Profiles.Cool.ApplyProfile();
                    break;
                case ProfileMode.Advanced:
                    Profiles.GenerateAdvancedProfile(Temperature, Tint, Saturation).ApplyProfile();
                    break;
                case ProfileMode.NightLight:
                    // Do nothing
                    break;
                default:
                    Profiles.Default.ApplyProfile();
                    break;
            }

            return SetRegistrySZ("UserSettingSelectedProfile", ConvertProfileMode(profile));
        }

        private ProfileMode GetProfile()
        {
            if (GetRegistrySZ("UserSettingSelectedProfile", out string result))
            {
                return ConvertProfileMode(result);
            }

            return ProfileMode.Standard;
        }

        public double Temperature
        {
            get => GetTemperature();
            set
            {
                if (value == Temperature)
                    return;
                if (SetTemperature(value))
                {
                    Profiles.GenerateAdvancedProfile(Temperature, Tint, Saturation).ApplyProfile();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Temperature"));
                }
            }
        }

        private bool SetTemperature(double value)
        {
            return SetRegistry("UserSettingAdvancedTemperature", (uint)value);
        }

        private double GetTemperature()
        {
            if (GetRegistry("UserSettingAdvancedTemperature", out uint result))
            {
                return result;
            }

            return 50;
        }

        public double Tint
        {
            get => GetTint();
            set
            {
                if (value == Tint)
                    return;
                if (SetTint(value))
                {
                    Profiles.GenerateAdvancedProfile(Temperature, Tint, Saturation).ApplyProfile();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tint"));
                }
            }
        }

        private bool SetTint(double value)
        {
            return SetRegistry("UserSettingAdvancedTint", (uint)value);
        }

        private double GetTint()
        {
            if (GetRegistry("UserSettingAdvancedTint", out uint result))
            {
                return result;
            }

            return 50;
        }

        public double Saturation
        {
            get => GetSaturation();
            set
            {
                if (value == Saturation)
                    return;
                if (SetSaturation(value))
                {
                    Profiles.GenerateAdvancedProfile(Temperature, Tint, Saturation).ApplyProfile();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Saturation"));
                }
            }
        }

        private bool SetSaturation(double value)
        {
            return SetRegistry("UserSettingAdvancedSaturation", (uint)value);
        }

        private double GetSaturation()
        {
            if (GetRegistry("UserSettingAdvancedSaturation", out uint result))
            {
                return result;
            }

            return 25;
        }
    }
}
