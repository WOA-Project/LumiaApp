using RegistryRT;
using System;
using System.ComponentModel;

namespace GlanceScreen
{
    public enum TimeMode
    {
        Off = 0,   // Mode=0
        s30 = 1,   // Mode=3
        m15 = 2,   // Mode=2
        Always = 3 // Mode=1
    }

    public enum BackgroundPhoto
    {
        BlackAndWhite = 0, // SyncedBgConversionOptions=11
        Color = 1,         // SyncedBgConversionOptions=9
    }

    // NormalModeElements
    // 0 1 1 1  1 1 1 1
    //   | |      |
    //   | |      \_ Notification icons
    //   | \________ Background photo
    //   \__________ Date

    public enum DarkModeElements
    {
        NotificationIcons = 1 << 2,
        Date              = 1 << 6
    }

    // DarkModeElements
    // 0 1 0 0  1 1 1 1
    //   |        |
    //   |        \_ Notification icons
    //   \__________ Date

    public enum NormalModeElements
    {
        NotificationIcons = 1 << 2,
        BackgroundPhoto   = 1 << 5,
        Date              = 1 << 6
    }

    // ShowDetailedAppStatus
    // 0=Off
    // 1=On (Detailed App Status)

    // AlwaysOnInCharger
    // 0=Off
    // 1=On (Always show glance screen when charging)

    // DarkMode
    // 0=Off
    // 1=Anything else

    // DarkModeOverrideColor
    // Red: FF0000
    // Green: 00FF00
    // Blue: 0000FF
    // Hide: 000000

    public enum NightModeColor
    {
        Red = 0xFF0000,
        Green = 0x00FF00,
        Blue = 0x0000FF,
        Hide = 0x000000
    }

    // DarkModeStart (in minutes)
    // DarkModeEnd (in minutes)

    public enum NightModeType
    {
        Off = 0,
        Red = 1,
        Green = 2,
        Blue = 3,
        HideGlanceScreen = 4
    }

    public class SettingsModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Registry registry => LumiaApp.App.Registry;

#if ARM64
        private const string RegPath = @"SOFTWARE\WowAA32Node\OEM\Nokia\lpm";
#else
        private const string RegPath = @"SOFTWARE\OEM\Nokia\lpm";
#endif

        private bool forbidWrites = true;

        public void RefreshEverything()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Mode"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundPhoto"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundPhotoColorType"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Date"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NotificationIcons"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DetailedAppStatus"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlwaysShowWhenCharging"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NightMode"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StartTime"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EndTime"));

            forbidWrites = false;
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
            byte[] buffer = new byte[sizeof(uint)];

            bool status = registry.QueryValue(
                RegistryHive.HKEY_LOCAL_MACHINE,
                RegPath,
                ValueName,
                out RegistryType type,
                out buffer
            );

            if (status && type == RegistryType.Integer)
            {
                ValueData =  BitConverter.ToUInt32(buffer, 0);
            }

            return status;
        }

        public int Mode
        {
            get => (int)GetMode();
            set
            {
                if (value == Mode)
                    return;
                if (SetMode((TimeMode)value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Mode"));
            }
        }

        private bool SetMode(TimeMode mode)
        {
            switch (mode)
            {
                case TimeMode.Off:
                    {
                        return SetRegistry("Mode", 0);
                    }
                case TimeMode.Always:
                    {
                        return SetRegistry("Mode", 1);
                    }
                case TimeMode.m15:
                    {
                        return SetRegistry("Mode", 2);
                    }
                case TimeMode.s30:
                    {
                        return SetRegistry("Mode", 3);
                    }
            }

            return false;
        }

        private TimeMode GetMode()
        {
            if (GetRegistry("Mode", out uint result))
            {
                switch (result)
                {
                    case 0:
                        return TimeMode.Off;
                    case 1:
                        return TimeMode.Always;
                    case 2:
                        return TimeMode.m15;
                    case 3:
                        return TimeMode.s30;
                }
            }

            return TimeMode.Off;
        }

        public bool BackgroundPhoto
        {
            get => GetBackgroundPhoto();
            set
            {
                if (value == BackgroundPhoto)
                    return;
                if (SetBackgroundPhoto(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundPhoto"));
            }
        }

        private bool SetBackgroundPhoto(bool enabled)
        {
            if (GetRegistry("NormalModeElements", out uint data))
            {
                if (enabled)
                {
                    data |= (uint)NormalModeElements.BackgroundPhoto;
                }
                else
                {
                    data &= ~(uint)NormalModeElements.BackgroundPhoto;
                }
                return SetRegistry("NormalModeElements", data);
            }

            return false;
        }

        private bool GetBackgroundPhoto()
        {
            if (GetRegistry("NormalModeElements", out uint data))
            {
                return (data & (uint)NormalModeElements.BackgroundPhoto) != 0;
            }

            return false;
        }

        public int BackgroundPhotoColorType
        {
            get => (int)GetBackgroundPhotoColorType();
            set
            {
                if (value == BackgroundPhotoColorType)
                    return;
                if (SetBackgroundPhotoColorType((BackgroundPhoto)value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundPhotoColorType"));
            }
        }

        private bool SetBackgroundPhotoColorType(BackgroundPhoto type)
        {
            switch (type)
            {
                case GlanceScreen.BackgroundPhoto.BlackAndWhite:
                    {
                        return SetRegistry("SyncedBgConversionOptions", 11);
                    }
                case GlanceScreen.BackgroundPhoto.Color:
                    {
                        return SetRegistry("SyncedBgConversionOptions", 9);
                    }
            }

            return false;
        }

        private BackgroundPhoto GetBackgroundPhotoColorType()
        {
            if (GetRegistry("SyncedBgConversionOptions", out uint data))
            {
                switch (data)
                {
                    case 11:
                        return GlanceScreen.BackgroundPhoto.BlackAndWhite;
                    case 9:
                        return GlanceScreen.BackgroundPhoto.Color;
                }
            }

            return GlanceScreen.BackgroundPhoto.BlackAndWhite;
        }

        public bool Date
        {
            get => GetDate();
            set
            {
                if (value == Date)
                    return;
                if (SetDate(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Date"));
            }
        }

        private bool SetDate(bool enabled)
        {
            if (GetRegistry("NormalModeElements", out uint data))
            {
                if (enabled)
                {
                    data |= (uint)NormalModeElements.Date;
                }
                else
                {
                    data &= ~(uint)NormalModeElements.Date;
                }
                if (SetRegistry("NormalModeElements", data) && GetRegistry("DarkModeElements", out data))
                {
                    if (enabled)
                    {
                        data |= (uint)NormalModeElements.Date;
                    }
                    else
                    {
                        data &= ~(uint)NormalModeElements.Date;
                    }
                    return SetRegistry("DarkModeElements", data);
                }
            }

            return false;
        }

        private bool GetDate()
        {
            if (GetRegistry("NormalModeElements", out uint data))
            {
                return (data & (uint)NormalModeElements.Date) != 0;
            }

            return false;
        }

        public bool NotificationIcons
        {
            get => GetNotificationIcons();
            set
            {
                if (value == NotificationIcons)
                    return;
                if (SetNotificationIcons(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NotificationIcons"));
            }
        }

        private bool SetNotificationIcons(bool enabled)
        {
            if (GetRegistry("NormalModeElements", out uint data))
            {
                if (enabled)
                {
                    data |= (uint)NormalModeElements.NotificationIcons;
                }
                else
                {
                    data &= ~(uint)NormalModeElements.NotificationIcons;
                }
                if (SetRegistry("NormalModeElements", data) && GetRegistry("DarkModeElements", out data))
                {
                    if (enabled)
                    {
                        data |= (uint)NormalModeElements.NotificationIcons;
                    }
                    else
                    {
                        data &= ~(uint)NormalModeElements.NotificationIcons;
                    }
                    return SetRegistry("DarkModeElements", data);
                }
            }

            return false;
        }

        private bool GetNotificationIcons()
        {
            if (GetRegistry("NormalModeElements", out uint data))
            {
                return (data & (uint)NormalModeElements.NotificationIcons) != 0;
            }

            return false;
        }

        public bool DetailedAppStatus
        {
            get => GetDetailedAppStatus();
            set
            {
                if (value == DetailedAppStatus)
                    return;
                if (SetDetailedAppStatus(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DetailedAppStatus"));
            }
        }

        private bool SetDetailedAppStatus(bool enabled)
        {
            return SetRegistry("ShowDetailedAppStatus", enabled ? 1u : 0u);
        }

        private bool GetDetailedAppStatus()
        {
            if (GetRegistry("ShowDetailedAppStatus", out uint data))
            {
                return data == 1;
            }

            return false;
        }

        public bool AlwaysShowWhenCharging
        {
            get => GetAlwaysShowWhenCharging();
            set
            {
                if (value == AlwaysShowWhenCharging)
                    return;
                if (SetAlwaysShowWhenCharging(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlwaysShowWhenCharging"));
            }
        }

        private bool SetAlwaysShowWhenCharging(bool enabled)
        {
            return SetRegistry("AlwaysOnInCharger", enabled ? 1u : 0u);
        }

        private bool GetAlwaysShowWhenCharging()
        {
            if (GetRegistry("AlwaysOnInCharger", out uint data))
            {
                return data == 1;
            }

            return false;
        }

        public int NightMode
        {
            get => (int)GetNightMode();
            set
            {
                if (value == NightMode)
                    return;
                if (SetNightMode((NightModeType)value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NightMode"));
            }
        }

        private bool SetNightMode(NightModeType mode)
        {
            switch (mode)
            {
                case NightModeType.Off:
                    {
                        return SetRegistry("DarkMode", 0u) && SetRegistry("SwitchOffInDarkMode", 0);
                    }
                case NightModeType.Blue:
                    {
                        if (SetRegistry("DarkMode", 1u))
                        {
                            return SetRegistry("DarkModeOverrideColor", (uint)NightModeColor.Blue) && SetRegistry("SwitchOffInDarkMode", 0);
                        }
                        break;
                    }
                case NightModeType.Green:
                    {
                        if (SetRegistry("DarkMode", 1u))
                        {
                            return SetRegistry("DarkModeOverrideColor", (uint)NightModeColor.Green) && SetRegistry("SwitchOffInDarkMode", 0);
                        }
                        break;
                    }
                case NightModeType.Red:
                    {
                        if (SetRegistry("DarkMode", 1u))
                        {
                            return SetRegistry("DarkModeOverrideColor", (uint)NightModeColor.Red) && SetRegistry("SwitchOffInDarkMode", 0);
                        }
                        break;
                    }
                case NightModeType.HideGlanceScreen:
                    {
                        if (SetRegistry("DarkMode", 1u))
                        {
                            return SetRegistry("DarkModeOverrideColor", (uint)NightModeColor.Hide) && SetRegistry("SwitchOffInDarkMode", 1);
                        }
                        break;
                    }
            }

            return false;
        }

        private NightModeType GetNightMode()
        {
            if (GetRegistry("DarkMode", out uint data))
            {
                if (data == 0)
                    return NightModeType.Off;

                if (GetRegistry("DarkModeOverrideColor", out data))
                {
                    switch (data)
                    {
                        case 0x0000FF:
                            if (!GetRegistry("SwitchOffInDarkMode", out data) || data == 0)
                                return NightModeType.Blue;
                            break;
                        case 0x00FF00:
                            if (!GetRegistry("SwitchOffInDarkMode", out data) || data == 0)
                                return NightModeType.Green;
                            break;
                        case 0xFF0000:
                            if (!GetRegistry("SwitchOffInDarkMode", out data) || data == 0)
                                return NightModeType.Red;
                            break;
                        case 0x000000:
                            if (GetRegistry("SwitchOffInDarkMode", out data) && data == 1)
                                return NightModeType.HideGlanceScreen;
                            break;
                    }
                }
            }

            return NightModeType.Off;
        }

        public TimeSpan StartTime
        {
            get => GetStartTime();
            set
            {
                if (value == StartTime)
                    return;
                if (SetStartTime(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StartTime"));
            }
        }

        private bool SetStartTime(TimeSpan time)
        {
            return SetRegistry("DarkModeStart", (uint)time.TotalMinutes);
        }

        private TimeSpan GetStartTime()
        {
            GetRegistry("DarkModeStart", out uint data);
            return new TimeSpan(0, (int)data, 0);
        }

        public TimeSpan EndTime
        {
            get => GetEndTime();
            set
            {
                if (value == EndTime)
                    return;
                if (SetEndTime(value))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EndTime"));
            }
        }

        private bool SetEndTime(TimeSpan time)
        {
            return SetRegistry("DarkModeEnd", (uint)time.TotalMinutes);
        }

        private TimeSpan GetEndTime()
        {
            GetRegistry("DarkModeEnd", out uint data);
            return new TimeSpan(0, (int)data, 0);
        }
    }
}
