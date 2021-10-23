using System;
using RegistryRT;

namespace ColorProfile
{
    public class Profile
    {
        private Registry regrt => LumiaApp.App.Registry;

        // Color values
        public double UserSettingColorTargetBlueX { get; set; }
        public double UserSettingColorTargetBlueY { get; set; }
        public double UserSettingColorTargetBlueZ { get; set; }
        public double UserSettingColorTargetGreenX { get; set; }
        public double UserSettingColorTargetGreenY { get; set; }
        public double UserSettingColorTargetGreenZ { get; set; }
        public double UserSettingColorTargetRedX { get; set; }
        public double UserSettingColorTargetRedY { get; set; }
        public double UserSettingColorTargetRedZ { get; set; }

        // White balance values
        public double UserSettingColorTargetWhiteX { get; set; }
        public double UserSettingColorTargetWhiteY { get; set; }
        public double UserSettingColorTargetWhiteZ { get; set; }

        public uint UserSettingColorSaturationMatrix { get; set; }
        public uint UserSettingColorSaturationPA { get; set; }

        private string key = @"SOFTWARE\OEM\Nokia\Display\ColorAndLight";

        public Profile()
        {

        }

        private void SetValue(string name, double value)
        {
            regrt.WriteValue(RegistryHive.HKEY_LOCAL_MACHINE, key, name, System.Text.Encoding.Unicode.GetBytes(Math.Round(value, 6).ToString() + "\0"), RegistryType.String);
        }

        private void SetValue(string name, uint value)
        {
            regrt.WriteValue(RegistryHive.HKEY_LOCAL_MACHINE, key, name, BitConverter.GetBytes(value), RegistryType.Integer);
        }

        private bool GetRegistry(string ValueName, out uint ValueData)
        {
            ValueData = 0;
            byte[] buffer;

            bool status = regrt.QueryValue(
                RegistryHive.HKEY_LOCAL_MACHINE,
                key,
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

        public void ApplyProfile()
        {
            SetValue("UserSettingColorTargetBlueX", UserSettingColorTargetBlueX);
            SetValue("UserSettingColorTargetBlueY", UserSettingColorTargetBlueY);
            SetValue("UserSettingColorTargetBlueZ", UserSettingColorTargetBlueZ);

            SetValue("UserSettingColorTargetGreenX", UserSettingColorTargetGreenX);
            SetValue("UserSettingColorTargetGreenY", UserSettingColorTargetGreenY);
            SetValue("UserSettingColorTargetGreenZ", UserSettingColorTargetGreenZ);

            SetValue("UserSettingColorTargetRedX", UserSettingColorTargetRedX);
            SetValue("UserSettingColorTargetRedY", UserSettingColorTargetRedY);
            SetValue("UserSettingColorTargetRedZ", UserSettingColorTargetRedZ);

            SetValue("UserSettingColorTargetWhiteX", UserSettingColorTargetWhiteX);
            SetValue("UserSettingColorTargetWhiteY", UserSettingColorTargetWhiteY);
            SetValue("UserSettingColorTargetWhiteZ", UserSettingColorTargetWhiteZ);

            SetValue("UserSettingColorSaturationMatrix", UserSettingColorSaturationMatrix);
            SetValue("UserSettingColorSaturationPA", UserSettingColorSaturationPA);

            uint val = 0u;
            GetRegistry("UserSettingAtomicUpdate", out val);
            SetValue("UserSettingAtomicUpdate", val == 1u ? 0u : 1u);
        }
    }

    public static class Profiles
    {
        public static Profile Default = new()
        {
            UserSettingColorTargetBlueX = 0.1805,
            UserSettingColorTargetBlueY = 0.0722,
            UserSettingColorTargetBlueZ = 0.9505,

            UserSettingColorTargetGreenX = 0.3576,
            UserSettingColorTargetGreenY = 0.7152,
            UserSettingColorTargetGreenZ = 0.1192,

            UserSettingColorTargetRedX = 0.4123,
            UserSettingColorTargetRedY = 0.2126,
            UserSettingColorTargetRedZ = 0.0192,

            UserSettingColorTargetWhiteX = 0.95015469,
            UserSettingColorTargetWhiteY = 1.0,
            UserSettingColorTargetWhiteZ = 1.08825906,

            UserSettingColorSaturationMatrix = 0x64,
            UserSettingColorSaturationPA = 0x64
        };

        public static Profile Cool = new()
        {
            UserSettingColorTargetBlueX = 0.1822,
            UserSettingColorTargetBlueY = 0.0677,
            UserSettingColorTargetBlueZ = 1.0105,

            UserSettingColorTargetGreenX = 0.3013,
            UserSettingColorTargetGreenY = 0.7206,
            UserSettingColorTargetGreenZ = 0.08235,

            UserSettingColorTargetRedX = 0.49245,
            UserSettingColorTargetRedY = 0.25015,
            UserSettingColorTargetRedZ = 0.0099,

            UserSettingColorTargetWhiteX = 0.950697,
            UserSettingColorTargetWhiteY = 1.0,
            UserSettingColorTargetWhiteZ = 1.31576,

            UserSettingColorSaturationMatrix = 0x64,
            UserSettingColorSaturationPA = 0x64
        };

        public static Profile Vivid = new()
        {
            UserSettingColorTargetBlueX = 0.18305,
            UserSettingColorTargetBlueY = 0.06545,
            UserSettingColorTargetBlueZ = 1.0405,

            UserSettingColorTargetGreenX = 0.27315,
            UserSettingColorTargetGreenY = 0.7233,
            UserSettingColorTargetGreenZ = 0.063925,

            UserSettingColorTargetRedX = 0.532475,
            UserSettingColorTargetRedY = 0.268925,
            UserSettingColorTargetRedZ = 0.0052,

            UserSettingColorTargetWhiteX = 0.949097,
            UserSettingColorTargetWhiteY = 1,
            UserSettingColorTargetWhiteZ = 1.15958,

            UserSettingColorSaturationMatrix = 0x67,
            UserSettingColorSaturationPA = 0x65
        };

        public static Profile GetNightLightProfile(double NightLightValue)
        {
            Profile prof = new()
            {
                UserSettingColorTargetBlueX = 0.1805,
                UserSettingColorTargetBlueY = 0.0722,
                UserSettingColorTargetBlueZ = 0.9505,

                UserSettingColorTargetGreenX = 0.3576,
                UserSettingColorTargetGreenY = 0.7152,
                UserSettingColorTargetGreenZ = 0.1192,

                UserSettingColorTargetRedX = 0.4123,
                UserSettingColorTargetRedY = 0.2126,
                UserSettingColorTargetRedZ = 0.0192,

                UserSettingColorSaturationMatrix = 0x64,
                UserSettingColorSaturationPA = 0x64
            };

            double Temperature = (NightLightValue * -212.24 + 26056) / 2;
            double Tint = 50;

            prof.UserSettingColorTargetWhiteX = -0.0003 * Temperature + 0.885784 + (-0.00000068 * Temperature + 0.001968) * Tint;
            prof.UserSettingColorTargetWhiteY = 1;
            prof.UserSettingColorTargetWhiteZ = 0.008406 * Temperature + 0.542142 + (0.0000187 * Temperature + 0.001205) * Tint;

            return prof;
        }

        public static Profile GenerateAdvancedProfile(double Temperature, double Tint, double Saturation)
        {
            Profile prof = new();

            prof.UserSettingColorSaturationMatrix = 0x64;
            prof.UserSettingColorSaturationPA = 0x64;

            if (Saturation > 66)
            {
                prof.UserSettingColorSaturationMatrix = 0x69;
                prof.UserSettingColorSaturationPA = 0x66;
            }

            prof.UserSettingColorTargetWhiteX = -0.0003 * Temperature + 0.885784 + (-0.00000068 * Temperature + 0.001968) * Tint;
            prof.UserSettingColorTargetWhiteY = 1;
            prof.UserSettingColorTargetWhiteZ = 0.008406 * Temperature + 0.542142 + (0.0000187 * Temperature + 0.001205) * Tint;

            if (Saturation < 50d)
            {
                prof.UserSettingColorTargetBlueX = 0.000068 * Saturation + 0.1788;
                prof.UserSettingColorTargetBlueY = -0.00018 * Saturation + 0.0767;
                prof.UserSettingColorTargetBlueZ = 0.0024 * Saturation + 0.8905;

                prof.UserSettingColorTargetGreenX = -0.00225 * Saturation + 0.4139;
                prof.UserSettingColorTargetGreenY = 0.000216 * Saturation + 0.7098;
                prof.UserSettingColorTargetGreenZ = -0.00147 * Saturation + 0.15605;

                prof.UserSettingColorTargetRedX = 0.003202 * Saturation + 0.33235;
                prof.UserSettingColorTargetRedY = 0.001502 * Saturation + 0.17505;
                prof.UserSettingColorTargetRedZ = -0.00038 * Saturation + 0.0287;
            }
            else
            {
                prof.UserSettingColorTargetBlueX = 0.000034 * (Saturation - 50d) + 0.1822;
                prof.UserSettingColorTargetBlueY = -0.00009 * (Saturation - 50d) + 0.0677;
                prof.UserSettingColorTargetBlueZ = 0.0012   * (Saturation - 50d) + 1.0105;

                prof.UserSettingColorTargetGreenX = -0.00113 * (Saturation - 50d) + 0.3013;
                prof.UserSettingColorTargetGreenY = 0.000108 * (Saturation - 50d) + 0.7206;
                prof.UserSettingColorTargetGreenZ = -0.00074 * (Saturation - 50d) + 0.08235;

                prof.UserSettingColorTargetRedX = 0.001601 * (Saturation - 50d) + 0.49245;
                prof.UserSettingColorTargetRedY = 0.000751 * (Saturation - 50d) + 0.25015;
                prof.UserSettingColorTargetRedZ = -0.00019 * (Saturation - 50d) + 0.0099;
            }

            return prof;
        }
    }
}